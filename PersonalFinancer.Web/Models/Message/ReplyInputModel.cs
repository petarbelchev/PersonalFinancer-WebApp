namespace PersonalFinancer.Web.Models.Message
{
    using PersonalFinancer.Common.Messages;
    using System.ComponentModel.DataAnnotations;

    public class ReplyInputModel
	{
		[Required]
		public string Id { get; set; } = null!;

		[Required(ErrorMessage = ValidationMessages.RequiredProperty)]
		[StringLength(1000, MinimumLength = 10, 
			ErrorMessage = ValidationMessages.InvalidLength)]
        public string ReplyContent { get; set; } = null!;
    }
}
