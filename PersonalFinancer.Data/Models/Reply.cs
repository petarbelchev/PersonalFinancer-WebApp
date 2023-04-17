namespace PersonalFinancer.Data.Models
{
	public class Reply
	{
        public string AuthorId { get; set; } = null!;

        public string AuthorName { get; set; } = null!;

        public DateTime CreatedOn { get; set; }

		public string Content { get; set; } = null!;
	}
}
