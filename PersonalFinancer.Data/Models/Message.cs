namespace PersonalFinancer.Data.Models
{
	using MongoDB.Bson;
	using MongoDB.Bson.Serialization.Attributes;

	public class Message
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; } = null!;

		public string AuthorId { get; set; } = null!;

        public string AuthorName { get; set; } = null!;

        public DateTime CreatedOn { get; set; }

        public string Subject { get; set; } = null!;

        public string Content { get; set; } = null!;

        public ICollection<Reply> Replies { get; set; } 
			= new List<Reply>();
    }
}
