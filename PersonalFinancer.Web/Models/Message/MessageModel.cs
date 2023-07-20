namespace PersonalFinancer.Web.Models.Message
{
	using PersonalFinancer.Common.Messages;
	using System.ComponentModel.DataAnnotations;
	using static PersonalFinancer.Common.Constants.MessageConstants;

	public class MessageModel
	{
		[Required(ErrorMessage = ValidationMessages.RequiredProperty)]
		[StringLength(MessageSubjectMaxLength, 
			MinimumLength = MessageSubjectMinLength, 
			ErrorMessage = ValidationMessages.InvalidLength)]
		public string Subject { get; set; } = null!;
		
		[Required(ErrorMessage = ValidationMessages.RequiredProperty)]
		[StringLength(MessageContentMaxLength, 
			MinimumLength = MessageContentMinLength, 
			ErrorMessage = ValidationMessages.InvalidLength)]
		public string Content { get; set; } = null!;
    }
}
