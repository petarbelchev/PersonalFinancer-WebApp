namespace PersonalFinancer.Data.Models
{
	using PersonalFinancer.Data.Models.Contracts;

	public class Message : BaseMongoDocument
    {
		public Message()
		{
            this.CreatedOnUtc = DateTime.UtcNow;
			this.IsSeenByAuthor = true;
		}

		public string AuthorName { get; set; } = null!;

        public DateTime CreatedOnUtc { get; set; }

        public string Subject { get; set; } = null!;

        public string Content { get; set; } = null!;

        public bool IsSeenByAuthor { get; set; }

        public bool IsSeenByAdmin { get; set; }

        public bool IsArchivedByAdmin { get; set; }

        public bool IsArchivedByAuthor { get; set; }

        public ICollection<Reply> Replies { get; set; }
           = new List<Reply>();

		public byte[]? Image { get; set; }
	}
}
