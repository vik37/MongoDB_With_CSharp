using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Mongo_DataAccess.Models;

public class ChoreHistoryModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string ChoreId { get; set; }
    public string ChoreText { get; set; }
    public DateTime? DateCompleted { get; set; }
    public UserModel WhoCompleted { get; set; }

    public ChoreHistoryModel()
    { }

    public ChoreHistoryModel(ChoreModel choreModel)
    {
        ChoreId = choreModel.Id;
        DateCompleted = choreModel.LastCompleted ?? DateTime.Now;
        WhoCompleted = choreModel.AssignTo;
        ChoreText = choreModel.ChoreText;
    }
}