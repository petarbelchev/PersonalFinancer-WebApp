namespace PersonalFinancer.Web.Models.Message
{
	using PersonalFinancer.Common.Messages;
	using System.ComponentModel.DataAnnotations;
	using static PersonalFinancer.Common.Constants.MessageConstants;

	public class ReplyInputModel
	{
		[Required]
		[RegularExpression("^[0-9A-Fa-f]{24}$")]
		public string MessageId { get; set; } = null!;

		[Required(ErrorMessage = ValidationMessages.RequiredProperty)]
		[StringLength(ReplyMaxLength, 
			MinimumLength = ReplyMinLength, 
			ErrorMessage = ValidationMessages.InvalidLength)]
        public string ReplyContent { get; set; } = null!;
    }
}
