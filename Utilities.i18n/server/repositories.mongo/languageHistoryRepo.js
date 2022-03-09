const { db, ObjectId } = require('./mongoRepoBase')
const phrasesHistoryCollection = db.collection('phrasesHistory');

const insert = async (phrase, action, session) => {
    phrase.originalId = phrase._id;
    phrase.action = action;
    delete phrase._id;
    await phrasesHistoryCollection.insertOne(phrase, { session });

}

const get = async (originalId) => {
    return await phrasesHistoryCollection.find({ originalId: new ObjectId(originalId) }).sort({_id:-1}).toArray();
}

const getDeleted = async () => {
    return await phrasesHistoryCollection.aggregate([
        {
            $match: { action: 'Deleted' }
        },
        {
            $sort: { _id: -1 }
        },
        {
            $addFields:
            {
                _id: '$originalId'
            }
        }

    ]).toArray();
}

module.exports = { insert, get, getDeleted };