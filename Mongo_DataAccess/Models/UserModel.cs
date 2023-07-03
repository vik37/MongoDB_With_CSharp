using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Mongo_DataAccess.Models;

public class UserModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}
