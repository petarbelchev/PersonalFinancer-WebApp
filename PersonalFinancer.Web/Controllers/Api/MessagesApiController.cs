namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Services.Messages;
	using PersonalFinancer.Services.Messages.Models;
	using PersonalFinancer.Web.CustomAttributes;
	using PersonalFinancer.Web.Extensions;
	using PersonalFinancer.Web.Models.Message;

	[Authorize]
	[Route("api/messages")]
	[ApiController]
	public class MessagesApiController : ControllerBase
	{
		private readonly IMessagesService messagesService;

		public MessagesApiController(IMessagesService messagesService)
			=> this.messagesService = messagesService;

		[HttpGet("{page}")]
		public async Task<IActionResult> AllMessages(int page)
		{
			MessagesDTO messagesDTO = this.User.IsAdmin()
				? await this.messagesService.GetAllAsync(page)
				: await this.messagesService.GetUserMessagesAsync(this.User.Id(), page);

			var model = new MessagesViewModel(messagesDTO, page);

			return this.Ok(model);
		}

		[HttpPatch("{messageId}")]
		[NotRequireHtmlEncoding]
		public async Task<IActionResult> MarkAsSeen(string messageId)
		{
			try
			{
				await this.messagesService.MarkMessageAsSeenAsync(messageId, this.User.Id(), this.User.IsAdmin());
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}

			return this.Ok();
		}
	}
}
