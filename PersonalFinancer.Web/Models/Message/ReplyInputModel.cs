namespace PersonalFinancer.Web.Models.Message
{
	using PersonalFinancer.Common.Messages;
	using System.ComponentModel.DataAnnotations;
	using static PersonalFinancer.Common.Constants.MessageConstants;

	public class ReplyInputModel
	{
		[Required]
		public string MessageId { get; set; } = null!;

		[Required(ErrorMessage = ValidationMessages.RequiredProperty)]
		[StringLength(ReplyMaxLength, MinimumLength = ReplyMinLength, 
			ErrorMessage = ValidationMessages.InvalidLength)]
        public string ReplyContent { get; set; } = null!;
    }
}
