const { db, ObjectId, createSession } = require('./mongoRepoBase');
const authorize = require('./../applicational/privileges');
const history = require('./languageHistoryRepo');
const cache = require('./cache');

const phrasesCollection = db.collection('phrases');

//#region public APIs
const add = async (phrase, email, role) => {

    if (!authorize('language.add', role)) {
        throw new Error('You do not have sufficient privilege to create a new phrase');
    }
    if (!phrase.key) {
        throw new Error('Please enter a key')
    }
    if (!phrase.en || !phrase.ar) {
        throw new Error('Please enter both english and arabic parts of the phrase');
    }
    if (!Array.isArray(phrase.usage) || !phrase.usage.length) {
        throw new Error('Phrase should have atleast one platform usage')
    }
    if (await exists(phrase.key, phrase.usage)) {
        throw new Error('key already exists in one of the usage parameters');
    }

    let date = new Date();
    phrase.createdAt = date;
    phrase.modifiedAt = date;
    phrase.exported = false;
    phrase.translated = false;
    phrase.createdBy = email;
    phrase.modifiedBy = email;

    //clearing cache
    cache.invalidate(phrase.usage);


    const session = createSession();
    await session.withTransaction(async () => {
        try {
            await phrasesCollection.insertOne(phrase);
            await history.insert(phrase, "Added", session);
            await session.commitTransaction();
        }
        catch (err) {
            await session.abortTransaction();
            throw err;
        }
        finally {
            session.endSession();
        }

    })

    phrase._id = phrase.originalId;
    return phrase;
}

const update = async (phrase, email, role) => {

    let original = await phrasesCollection.findOne({ _id: new ObjectId(phrase._id) });
    let translated = original.translated;
    let anyChangeInLangPart = false;

    if (!areArraysEqual(original.usage, phrase.usage) && !authorize('language.update.usage', role)) {
        throw new Error('You do not have sufficient privilege to update usage of the phrase');
    }

    if (original.en !== phrase.en) {
        anyChangeInLangPart = true;
        if (!authorize('language.update.en', role))
            throw new Error('You do not have sufficient privilege to update english part of the phrase');

        if (role === 'developer')
            translated = false;
    }

    if (original.ar !== phrase.ar) {
        anyChangeInLangPart = true;
        if (!authorize('language.update.ar', role))
            throw new Error('You do not have sufficient privilege to update arabic part of the phrase');

        if (role === 'translator')
            translated = true;
    }

    //validations
    if (!original.key) {
        throw new Error('Please enter a key')
    }
    if (!phrase.en || !phrase.ar) {
        throw new Error('Please enter both english and arabic phrases');
    }
    if (!Array.isArray(phrase.usage) || !phrase.usage.length) {
        throw new Error('Phrase should have atleast one platform usage')
    }

    //exists check
    if (await exists(original.key, phrase.usage, phrase._id)) {
        throw new Error('key already exists in one of the usage parameters')
    }



    let setDoc = {
        $set: {
            modifiedAt: new Date(),
            ar: phrase.ar,
            en: phrase.en,
            usage: phrase.usage,
            modifiedBy: email,
            translated: translated
        }
    }
    //clearing cache
    if (anyChangeInLangPart) {
        cache.invalidate([...phrase.usage, ...original.usage]);
    }
    else {
        cache.invalidate(diffArray(phrase.usage, original.usage));
    }

    //starting new transaction
    const session = createSession();
    await session.withTransaction(async () => {

        try {

            await phrasesCollection.updateOne({ _id: ObjectId(phrase._id) }, setDoc, { session });

            await history.insert(Object.assign(original, setDoc.$set), "Updated", session);

            await session.commitTransaction();
        }
        catch (err) {
            await session.abortTransaction();
            throw err;
        }
        finally {
            session.endSession();
        }

    });
    original._id = original.originalId;
    delete original.originalId;
    delete original.action;
    return original;
}

const search = async (filter/*send null or {} for no filter*/, page /*page starts with 1*/, size) => {
    page = +page;
    size = +size;

    const predicates = []
    let pipeline = [
        {
            $match:
            {
                $and: predicates
            },
        },
        {
            $sort: { _id: -1 }
        },
        {
            $skip: (page - 1) * size
        },
        {
            $limit: size
        }];

    if (filter && Object.keys(filter).length > 0) {
        filter.key && predicates.push({ key: filter.key });
        filter.ar && predicates.push({ ar: { $regex: filter.ar, $options: 'gim' } });
        filter.en && predicates.push({ en: { $regex: filter.en, $options: 'gim' } });
        Array.isArray(filter.usage) && filter.usage.length > 0 && predicates.push({ usage: { $in: filter.usage } });
        typeof filter.translated === 'boolean' && predicates.push({ translated: filter.translated });
    }
    if (predicates.length == 0) {
        pipeline.shift(); // if no filter applied, remove the $match(1st) stage from the pipeline
    }
    let result = phrasesCollection.aggregate(pipeline, { allowDiskUse: true });
    return await result.toArray();
}

const json = async (usage) => {
    let fromCache = cache.get(usage);
    let servedFrom = 'cache';
    if (!fromCache) { //cache is empty

        //get data from db  and fill the cache again
        let cursor = phrasesCollection.aggregate([
            {
                $match:
                {
                    usage: { $in: [usage] }
                }
            },
            {
                $project:
                {
                    key: 1,
                    ar: 1,
                    en: 1
                }
            }

        ]);
        let result = await cursor.toArray();
        let phrases = {};

        result.forEach(x => phrases[x.key] = { ar: x.ar, en: x.en });
        servedFrom = 'disk';
        cache.set(usage, phrases);
        fromCache = cache.get(usage);
    }
    console.log(`json served from ${servedFrom}`);
    return fromCache;
}

const getById = async (id) => {
    let filter = { _id: { $eq: new ObjectId(id) } };
    return await phrasesCollection.findOne(filter);
}

const getByKey = async (key) => {
    let filter = { key: key };
    return await phrasesCollection.find(filter).toArray();
}

const similarity = async (text) => {


    const pipeline = [
        {
            $match:
            {
                $text: { $search: text }
            }
        },
        {
            $sort: { score: { $meta: 'textScore' } }
        },
        {
            $project:
            {
                score: { $meta: 'textScore' },
                key: 1,
                usage: 1,
                en: 1,
                ar: 1
            }
        },
        {
            $limit: 10
        }];


    let cursor = phrasesCollection.aggregate(pipeline, { allowDiskUse: true });
    return await cursor.toArray();
}

const suspect = async () => {
    let pipeline = [
        {
            $match:
            {
                ar: { $not: { $regex: '[\u0600-\u06FF]', $options: 'um' } }
            }
        },
        {
            $sort: { _id: -1 }

        }];

    let cursor = phrasesCollection.aggregate(pipeline, { allowDiskUse: true });
    return await cursor.toArray();
}

const deletePhrase = async (id, role, email) => {

    if (!authorize('language.delete', role)) {
        throw new Error('You do not have sufficient privilege to delete a phrase.');
    }

    let original = await phrasesCollection.findOne({ _id: new ObjectId(id) });
    let diffTime = Math.abs(new Date() - original.createdAt);
    let diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

    if (original.translated == true) {
        throw new Error('Translated phrases cannot be deleted.');
    }
    if (diffDays > 30) {
        throw new Error('Deletion buffer time of 30 days has expired.')
    }
    //clearing cache
    cache.invalidate(original.usage);

    const session = createSession();
    await session.withTransaction(async () => {
        try {
            original.modifiedBy = email;
            original.modifiedAt = new Date();
            await history.insert(original, "Deleted", session);
            await phrasesCollection.deleteOne({ _id: ObjectId(id) });
            await session.commitTransaction();
        }
        catch (err) {
            await session.abortTransaction();
            throw err;
        }
        finally {
            session.endSession();
        }
    });

    return true;
}

const migrate = async (phrases, usage, email, role) => {

    if (!authorize('language.migrate', role)) {
        throw new Error('You do not have sufficient privilege to migrate phrases');
    }


    let failed = [];
    let batchSize = 1000;
    let batches = phrases.length / batchSize;
    let lastIndexProcessed = 0;
    batches = parseInt(batches > parseInt(batches) ? batches + 1 : batches);
    while (batches != 0) {
        let session = createSession();
        await session.withTransaction(async () => {

            let tobeInserted;
            try {

                let date = new Date();
                tobeInserted = phrases.slice(lastIndexProcessed, lastIndexProcessed + batchSize).map(p => {
                    return {
                        ...p, ...{
                            usage: [usage],
                            exported: true,
                            createdAt: date,
                            modifiedAt: date,
                            translated: true,
                            createdBy: email,
                            modifiedBy: email
                        }
                    };
                });

                await phrasesCollection.insertMany(tobeInserted);
                await session.commitTransaction();
            }
            catch (err) {
                await session.abortTransaction();
                failed.push(...tobeInserted);
            }
            finally {
                session.endSession();
                lastIndexProcessed = lastIndexProcessed + tobeInserted.length;
                batches = batches - 1;
            }

        });

    }
    return { batch: batchSize, total: phrases.length, exported: phrases.length - failed.length, failed: failed.length, faiures: failed };
}
//#endregion public APIs


//#region private fns


const exists = async (key, usage, id) => {

    let filter = { key: key, usage: { $in: usage } }
    if (id)
        filter = { ...filter, ...{ _id: { $ne: new ObjectId(id) } } };

    let cursor = phrasesCollection.find(filter);
    let docs = await cursor.toArray();
    return docs.length !== 0;
}
const areArraysEqual = (a, b) => {
    a = new Set(a);
    b = new Set(b);
    if (a.size !== b.size)
        return false;
    for (let i of a) {
        if (!b.has(i))
            return false;
    }
    return true;
}

const diffArray = (a, b) => {

    const aMinusb = (a, b) => {
        let result = [];
        for (let i = 0; i <= a.length; i++) {
            for (let j = 0; j <= b.length; j++) {
                if (a[i] === b[j]) {
                    break;
                }
                if (j == b.length) {
                    result.push(a[i]);
                }
            }
        }
        return result;
    }

    return [...aMinusb(a, b), ...aMinusb(b, a)];
}

//#endregion private fns
module.exports = { add, update, getById, getByKey, deletePhrase, search, json, migrate, similarity, suspect }
