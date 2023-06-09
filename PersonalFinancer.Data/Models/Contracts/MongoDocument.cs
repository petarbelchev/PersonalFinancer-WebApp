namespace PersonalFinancer.Data.Models.Contracts
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class MongoDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid AuthorId { get; set; }
    }
}
