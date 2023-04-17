namespace PersonalFinancer.Services.Messages.Models
{
    public class ReplyOutputServiceModel
    {
        public string AuthorName { get; set; } = null!;

        public DateTime CreatedOn { get; set; }

        public string Content { get; set; } = null!;
    }
}
