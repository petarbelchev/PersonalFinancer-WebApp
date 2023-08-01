namespace PersonalFinancer.Web.Models.Message
{
	using System.ComponentModel.DataAnnotations;
	using static PersonalFinancer.Common.Constants.MessageConstants;
	using static PersonalFinancer.Common.Messages.ValidationMessages;

	public class MessageModel
	{
		[Required(ErrorMessage = RequiredProperty)]
		[StringLength(MessageSubjectMaxLength, 
			MinimumLength = MessageSubjectMinLength, 
			ErrorMessage = InvalidLength)]
		public string Subject { get; set; } = null!;
		
		[Required(ErrorMessage = RequiredProperty)]
		[StringLength(MessageContentMaxLength, 
			MinimumLength = MessageContentMinLength, 
			ErrorMessage = InvalidLength)]
		public string Content { get; set; } = null!;

		public IFormFile? Image { get; set; }
	}
}
