namespace PersonalFinancer.Data.Models
{
    public class Reply
    {
		public Reply() 
            => this.CreatedOnUtc = DateTime.UtcNow;

		public string AuthorId { get; set; } = null!;

        public string AuthorName { get; set; } = null!;

        public DateTime CreatedOnUtc { get; set; }

        public string Content { get; set; } = null!;
    }
}
