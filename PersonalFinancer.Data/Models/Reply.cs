namespace PersonalFinancer.Data.Models
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class Reply
    {
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid AuthorId { get; set; }

        public string AuthorName { get; set; } = null!;

        public DateTime CreatedOn { get; set; }

        public string Content { get; set; } = null!;
    }
}
