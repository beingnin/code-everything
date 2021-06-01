using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canvas
{
    public class MongoTransactionalRepo
    {
        const string CONNECTIONSTRING = "mongodb://10.10.100.186:27017,10.10.100.187:27017,10.10.100.188:27017/learns?replicaSet=spsamongo";
        readonly IMongoClient _client;
        readonly IMongoDatabase _db;
        public MongoTransactionalRepo()
        {
            _client = new MongoClient(CONNECTIONSTRING);
            _db = _client.GetDatabase("learns");
        }

        public async Task Insert1(Person p, IClientSessionHandle session)
        {
            await _db.GetCollection<Person>("persons1").InsertOneAsync(session,p);
        }
        public async Task Insert2(Person p,IClientSessionHandle session)
        {
            await _db.GetCollection<Person>("persons2").InsertOneAsync(session,p);
        }
        public async Task RunTransaction(Person p)
        {
            var count = await _db.GetCollection<Person>("persons1").CountDocumentsAsync(FilterDefinition<Person>.Empty);
            Console.WriteLine("Count after: " + count);
            using (var session = await new MongoClient(CONNECTIONSTRING).StartSessionAsync(new ClientSessionOptions { CausalConsistency = true }))
            {
                try
                {
                    session.StartTransaction(new TransactionOptions(
                                             readConcern: ReadConcern.Snapshot,
                                             writeConcern: WriteConcern.WMajority));
                    await Insert1(p,session);
                    if (new Random().Next(1, 3) == 1) //simulating random failure
                        throw new InvalidOperationException("simulated error");
                    await Insert2(p,session);

                    await session.CommitTransactionAsync();
                    Console.WriteLine("Transaction committed");
                }
                catch (Exception ex)
                {
                    await session.AbortTransactionAsync();
                    Console.WriteLine("Transaction aborted");
                }
                count = await _db.GetCollection<Person>("persons1").CountDocumentsAsync(FilterDefinition<Person>.Empty);
                Console.WriteLine("Count after: "+count);
            }
        }
    }
}
