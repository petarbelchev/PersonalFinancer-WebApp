namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Services.Messages;
	using PersonalFinancer.Web.Extensions;

	[Authorize]
	[Route("api/messages")]
	[ApiController]
	public class MessagesApiController : ControllerBase
	{
		private readonly IMessagesService messagesService;

		public MessagesApiController(IMessagesService messagesService)
			=> this.messagesService = messagesService;

		[HttpPatch("{messageId}")]
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
