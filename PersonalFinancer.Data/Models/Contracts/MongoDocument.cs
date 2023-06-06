using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PersonalFinancer.Data.Models.Contracts
{
    public class MongoDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        public string AuthorId { get; set; } = null!;
    }
}
