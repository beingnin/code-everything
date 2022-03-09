const { db } = require('./firebaseAppBase')
const uuid = require('uuid')
const phrasesCollection = db.collection('phrases');
const historyCollection = db.collection('history');
const { similarity } = require('../helpers/utility')

// usage = { portal: 'p', egate: 'e', backend: 'b', mobile: 'm' }

const get = async () => {
    let snapshot = await phrasesCollection.get();
    return snapshot.docs.map(doc => {
        return { ...{ ref: doc.id }, ...doc.data() };
    });
}

const search = async (filter) => {

    let snapshot = phrasesCollection;
    if (filter) {
        if (filter.key) {
            snapshot = snapshot.where('key', '==', filter.key)
        }

        if (filter.en) {
            snapshot = snapshot.where('en', '==', filter.en)
        }

        if (filter.ar) {
            snapshot = snapshot.where('ar', '==', filter.ar)
        }

        if (filter.usage) {
            snapshot = snapshot.where('usage', 'array-contains', filter.usage)
        }

        if (filter.translated) {
            snapshot = snapshot.where('translated', '==', filter.translated)
        }
    }


    snapshot = await snapshot.orderBy('createdAt', 'desc').get();
    let result = snapshot.docs.map(doc => {
        return { ...{ ref: doc.id }, ...doc.data() };
    });

    return result;
}

const exportAsFile = async (usage) => {
    let result = await phrasesCollection.where('usage', 'array-contains', usage).select('key', 'ar', 'en').get();
    return result.docs.map(doc => {

        let r = {};
        let d =doc.data();
        r[d.key]={ar:d.ar,en:d.en};
        return r;
    });
}

const exists = async (key, usage, ref) => {
    let snapshot = phrasesCollection.where('usage', 'array-contains-any', usage).where('key', '==', key);
    if (ref) {
        snapshot = snapshot.where('__name__', '!=', ref);
    }
    snapshot = await snapshot.get();

    return snapshot.docs.length > 0;

}

const findByKey = async (key) => {
    let snapshot = await phrasesCollection.where("key", "==", key).get();

    return snapshot.docs.map(doc => {
        return { ...{ ref: doc.id }, ...doc.data() };
    });
}

const findByRef = async (ref) => {

    let snapshot = await phrasesCollection.where('__name__', '==', ref).get();
    return (snapshot.docs.map(doc => {
        return { ...{ ref: doc.id }, ...doc.data() };
    }))[0];
}

const add = async (phrase, email) => {

    if (await exists(phrase.key, phrase.usage)) {
        throw new Error('key already exists in one of the usage parameters')
    }

    phrase.id = uuid.v4();
    let date = new Date().getTime();
    phrase.createdAt = date;
    phrase.modifiedAt = date;
    phrase.exported = false;
    phrase.translated = false;
    phrase.createdBy = email;
    phrase.modifiedBy = email;
    let id = (await phrasesCollection.add(phrase)).id;
    phrase.ref = id;
    return phrase;
}

const update = async (phrase, email) => {

    let snapshot = await phrasesCollection.where('__name__', '==', phrase.ref).get();
    let original = (snapshot.docs.map(doc => {
        return { ...{ ref: doc.id }, ...doc.data() };
    }))[0];

    if (!phrase.ar || !phrase.en) {
        throw new Error('Language element not present')
    }

    if (await exists(original.key, phrase.usage, phrase.ref)) {
        throw new Error('key already exists in one of the usage parameters')
    }

    let doc = {
        modifiedAt: new Date().getTime(),
        ar: phrase.ar,
        en: phrase.en,
        usage: phrase.usage,
        modifiedBy: email
    };

    if (original.ar !== phrase.ar) {
        doc.translated = true;
    }
    let batch = db.batch();

    batch.update(phrasesCollection.doc(phrase.ref), doc);
    batch.set( historyCollection.doc(),{ ...original, ...{ operation: 'update', ref: phrase.ref } });
    await batch.commit();
    return phrase.ref;
}

const similar = async (en) => {

    let result = [];
    let snapshot = await phrasesCollection.get();

    snapshot.docs.forEach(document => {
        let d = document.data();
        let score = similarity(en, d.en);
        let doc = { ...{ ref: document.id, score: score }, ...d };

        if (result.length < 10 && score > 0.4) {
            result.push(doc);
        }
        else {

            for (let i = 0; i < result.length; i++) {
                if (score > result[i].score) {
                    result[i] = doc;
                    break;
                }
            }
        }
    });

    return result;


}

const migrate = async (phrases, usage, email) => {
    let batch = db.batch();
    let counter = 0;
    let totalCounter = 0;
    let date = new Date().getTime();

    const promises = [];

    for (let phrase of phrases) {

        let docRef = phrasesCollection.doc();

        phrase.id = uuid.v4();
        phrase.createdAt = date;
        phrase.modifiedAt = date;
        phrase.exported = true;
        phrase.translated = true;
        phrase.createdBy = email;
        phrase.modifiedBy = email;
        phrase.usage = [usage];
        batch.set(docRef, phrase);
        counter++;

        if (counter >= 500) {
            console.log('committing batch')
            promises.push(batch.commit());
            totalCounter += counter;
            counter = 0;
            batch = db.batch();
        }

    }


    if (counter) {
        console.log('committing batch')
        promises.push(batch.commit());
        totalCounter += counter;
    }

    await Promise.all(promises);
}



module.exports = { get, add, findByKey, findByRef, update, similar, search, exportAsFile, migrate }


//private functions

