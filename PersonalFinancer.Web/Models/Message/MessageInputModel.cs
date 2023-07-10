namespace PersonalFinancer.Web.Models.Message
{
    using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Web.CustomAttributes;
	using System.ComponentModel.DataAnnotations;
	using static PersonalFinancer.Common.Constants.MessageConstants;

    public class MessageInputModel
	{
		[Required(ErrorMessage = ValidationMessages.RequiredProperty)]
		[StringLength(MessageSubjectMaxLength, 
			MinimumLength = MessageSubjectMinLength, 
			ErrorMessage = ValidationMessages.InvalidLength)]
		[RequireHtmlEncoding]
		public string Subject { get; set; } = null!;
		
		[Required(ErrorMessage = ValidationMessages.RequiredProperty)]
		[StringLength(MessageContentMaxLength, 
			MinimumLength = MessageContentMinLength, 
			ErrorMessage = ValidationMessages.InvalidLength)]
		[RequireHtmlEncoding]
		public string Content { get; set; } = null!;
    }
}
