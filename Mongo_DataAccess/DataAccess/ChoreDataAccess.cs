using Mongo_DataAccess.Models;
using MongoDB.Driver;

namespace Mongo_DataAccess.DataAccess;

public class ChoreDataAccess
{
    private const string AtlasConnectionString = "**************";
    private const string ConnectionString = "mongodb://127.0.0.1:27017";
    private const string Database = "choredb";
    private const string ChoreCollection = "chore_chart";
    private const string UserCollection = "users";
    private const string HistoryChoreCollection = "chore_history";

    private IMongoCollection<T> ConnectToMongo<T>(in string collection)
    {
        var client = new MongoClient(ConnectionString);
        var db = client.GetDatabase(Database);
        return db.GetCollection<T>(collection);
    }

    public async Task<IEnumerable<UserModel>> GetAllUsers()
    {
        var userCollection = ConnectToMongo<UserModel>(UserCollection);
        var results = await userCollection.FindAsync(_ => true);

        return results.ToList();
    }

    public async Task<IEnumerable<ChoreModel>> GetAllChores()
    {
        var choreCollection = ConnectToMongo<ChoreModel>(ChoreCollection);
        var results = await choreCollection.FindAsync(_ => true);

        return results.ToList();
    }

    public async Task<IEnumerable<ChoreHistoryModel>> GetAllChoreHistory()
    {
        var historyChoreCollection = ConnectToMongo<ChoreHistoryModel>(HistoryChoreCollection);
        var results = await historyChoreCollection.FindAsync(_ => true);

        return results.ToList();
    }

    public async Task<IEnumerable<ChoreModel>> GetAllChoresForUser(UserModel user)
    {
        var choreCollection = ConnectToMongo<ChoreModel>(ChoreCollection);

        var results = await choreCollection.FindAsync(u => u.AssignTo.Id == user.Id);

        return results.ToList();
    }

    public Task CreateUser(UserModel user)
    {
        var userCollection = ConnectToMongo<UserModel>(UserCollection);
        return  userCollection.InsertOneAsync(user);
    }

    public Task CreateChore(ChoreModel chore)
    {
        var choreCollection = ConnectToMongo<ChoreModel>(ChoreCollection);
        return choreCollection.InsertOneAsync(chore);
    }

    public Task UpdateChore(ChoreModel chore)
    {
        var choreCollection = ConnectToMongo<ChoreModel>(ChoreCollection);
        var filter = Builders<ChoreModel>.Filter.Eq("Id",chore.Id);
        return choreCollection.ReplaceOneAsync(filter,chore,new ReplaceOptions { IsUpsert = true });
    }

    public Task DeleteChore(ChoreModel chore)
    {
        var choreCollection = ConnectToMongo<ChoreModel>(ChoreCollection);
        return choreCollection.DeleteOneAsync(c => c.Id == chore.Id);
    }

    public async Task CompleteChore(ChoreModel chore)
    {
        //var choreCollection = ConnectToMongo<ChoreModel>(ChoreCollection);
        //var filter = Builders<ChoreModel>.Filter.Eq("Id",chore.Id);
        //await choreCollection.ReplaceOneAsync(filter, chore);

        //var choreHistoryCollection = ConnectToMongo<ChoreHistoryModel>(HistoryChoreCollection);
        //await choreHistoryCollection.InsertOneAsync(new ChoreHistoryModel(chore));

        #region Transactions
        var client = new MongoClient(AtlasConnectionString);
        using var session = await client.StartSessionAsync();

        session.StartTransaction();

        try
        {
            var db = client.GetDatabase(Database);
            var choreCollection = db.GetCollection<ChoreModel>(ChoreCollection);

            var filter = Builders<ChoreModel>.Filter.Eq("Id", chore.Id);
            await choreCollection.ReplaceOneAsync(filter, chore);

            var choreHistoryCoolection = db.GetCollection<ChoreHistoryModel>(HistoryChoreCollection);
            await choreHistoryCoolection.InsertOneAsync(new ChoreHistoryModel(chore));

            await session.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync();
            await Console.Out.WriteLineAsync(ex.Message);
        }
        #endregion
    }
}
