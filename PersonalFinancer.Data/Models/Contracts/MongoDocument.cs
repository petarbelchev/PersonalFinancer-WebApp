namespace PersonalFinancer.Data.Models.Contracts
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public abstract class MongoDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        public string AuthorId { get; set; } = null!;
    }
}
