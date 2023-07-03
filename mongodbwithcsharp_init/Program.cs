using Mongo_DataAccess.DataAccess;
using Mongo_DataAccess.Models;
using MongoDB.Driver;

namespace mongodbwithcsharp_init;

internal class Program
{
    static async Task Main(string[] args)
    {
        #region MongoDB Init
            //string connectionString = "mongodb://127.0.0.1:27017";
            //string database = "simple_db";
            //string collectionName = "people";

            //var client = new MongoClient(connectionString);
            //var db = client.GetDatabase(database);
            //var collection = db.GetCollection<PersonModel>(collectionName);

            //var person = new PersonModel { FirstName = "Viktor", LastName = "Zafirovski" };

            //await collection.InsertOneAsync(person);

            //var results = await collection.FindAsync(_ => true);

            //foreach (var result in results.ToList())
            //{
            //    await Console.Out.WriteLineAsync($"{result.Id} {result.FirstName} {result.LastName}");
            //}
        #endregion

        ChoreDataAccess db = new ChoreDataAccess();
        await db.CreateUser(new UserModel { FirstName = "Viktor", LastName = "Zafirovski" });
        var users = await db.GetAllUsers();

        var chore = new ChoreModel 
        { 
            AssignTo = users.First(), 
            ChoreText = "Mow the Lown",
            FreuencyInDays = 7 
        };

        await db.CreateChore(chore);

        var chores = await db.GetAllChores();

        var newChore = chores.First();
        newChore.LastCompleted = DateTime.UtcNow;

        await db.CompleteChore(newChore);
    }
}