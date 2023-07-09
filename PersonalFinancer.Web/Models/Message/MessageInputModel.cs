namespace PersonalFinancer.Web.Models.Message
{
    using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Web.CustomAttributes;
	using System.ComponentModel.DataAnnotations;

    public class MessageInputModel
	{
		[Required(ErrorMessage = ValidationMessages.RequiredProperty)]
		[StringLength(50, MinimumLength = 10, 
			ErrorMessage = ValidationMessages.InvalidLength)]
		[RequireHtmlEncoding]
		public string Subject { get; set; } = null!;
		
		[Required(ErrorMessage = ValidationMessages.RequiredProperty)]
		[StringLength(1000, MinimumLength = 20, 
			ErrorMessage = ValidationMessages.InvalidLength)]
		[RequireHtmlEncoding]
		public string Content { get; set; } = null!;
    }
}
