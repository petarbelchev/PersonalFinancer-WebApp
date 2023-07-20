namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Services.Messages;
	using PersonalFinancer.Services.Messages.Models;
	using PersonalFinancer.Services.User;
	using PersonalFinancer.Web.CustomAttributes;
	using PersonalFinancer.Web.Extensions;
	using PersonalFinancer.Web.Models.Message;

	[Authorize]
	[Route("api/messages")]
	[ApiController]
	public class MessagesApiController : ControllerBase
	{
		private readonly IMessagesService messagesService;
		private readonly IUsersService usersService;

		public MessagesApiController(
			IMessagesService messagesService,
			IUsersService usersService)
		{
			this.messagesService = messagesService;
			this.usersService = usersService;
		}

		[HttpPost]
		[Produces("application/json")]
		[ProducesResponseType(typeof(ReplyOutputDTO), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> AddReply(ReplyInputModel inputModel)
		{
			if (!this.ModelState.IsValid)
				return this.BadRequest();

			string userId = this.User.Id();
			string userFullName = await this.usersService.UserFullNameAsync(Guid.Parse(userId));

			var dto = new ReplyInputDTO
			{
				MessageId = inputModel.MessageId,
				AuthorId = userId,
				AuthorName = userFullName,
				Content = inputModel.ReplyContent,
				IsAuthorAdmin = this.User.IsAdmin()
			};

			try
			{
				return this.Ok(await this.messagesService.AddReplyAsync(dto));
			}
			catch (ArgumentException)
			{
				return this.Unauthorized();
			}
			catch (InvalidOperationException)
			{
				return this.BadRequest();
			}			
		}

		[HttpGet]
		[Route("all/{page}")]
		[Produces("application/json")]
		[ProducesResponseType(typeof(MessagesViewModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> All(int page)
		{
			MessagesDTO messagesDTO = this.User.IsAdmin()
				? await this.messagesService.GetAllMessagesAsync(page)
				: await this.messagesService.GetUserMessagesAsync(this.User.Id(), page);

			return this.Ok(new MessagesViewModel(messagesDTO, page));
		}

		[HttpGet]
		[Route("archived/{page}")]
		[Produces("application/json")]
		[ProducesResponseType(typeof(MessagesViewModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> Archived(int page)
		{
			MessagesDTO messagesDTO = this.User.IsAdmin()
				? await this.messagesService.GetAllArchivedMessagesAsync(page)
				: await this.messagesService.GetUserArchivedMessagesAsync(this.User.Id(), page);

			return this.Ok(new MessagesViewModel(messagesDTO, page));
		}

		[HttpPatch("{messageId}")]
		[NoHtmlSanitizing]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
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
