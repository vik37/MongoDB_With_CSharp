using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mongo_DataAccess.Models;

public class ChoreModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string ChoreText { get; set; }
    public int FreuencyInDays { get; set; }
    public UserModel AssignTo { get; set; }
    public DateTime? LastCompleted { get; set; }
}
