const { MongoClient, ObjectId } = require('mongodb');

const uri = process.env.MongoURI;
const mongoOptions = {
    useNewUrlParser: true,
    useUnifiedTopology: true,
};

const client = new MongoClient(uri, mongoOptions);

client.on('serverClosed', (event) => { console.log('connection to mongodb server closed'); });
client.connect();

const db = client.db('i18n');

(async function () {

    try {
        await db.command({ ping: 1 })
    }
    catch (error) {
        console.error(error);
    }

})();

const createSession = () => {

    const opts = {
        readPreference: 'primary',
        readConcern: { level: 'local' },
        writeConcern: { w: 'majority' }
    };

    return client.startSession();

}
module.exports = { db, ObjectId, createSession };
