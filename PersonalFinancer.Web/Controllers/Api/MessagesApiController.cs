namespace PersonalFinancer.Web.Controllers.Api
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Services.Messages;
	using PersonalFinancer.Services.Messages.Models;
	using PersonalFinancer.Services.Users;
	using PersonalFinancer.Web.CustomAttributes;
	using PersonalFinancer.Web.Models.Message;
	using System.ComponentModel.DataAnnotations;

	[Authorize]
	[Route("api/messages")]
	[ApiController]
	public class MessagesApiController : ControllerBase
	{
		private readonly IMessagesService messagesService;
		private readonly IUsersService usersService;
		private readonly ILogger<MessagesApiController> logger;

		public MessagesApiController(
			IMessagesService messagesService,
			IUsersService usersService,
			ILogger<MessagesApiController> logger)
		{
			this.messagesService = messagesService;
			this.usersService = usersService;
			this.logger = logger;
		}

		[HttpPost]
		[Produces("application/json")]
		[ProducesResponseType(typeof(ReplyOutputDTO), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> AddReply(ReplyInputModel inputModel)
		{
			if (!this.ModelState.IsValid)
			{
				this.logger.LogWarning(
					LoggerMessages.AddReplyWithInvalidInputData,
					this.User.Id());

				return this.BadRequest();
			}

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
			catch (UnauthorizedAccessException)
			{
				this.logger.LogWarning(
					LoggerMessages.UnauthorizedReplyAddition,
					this.User.Id(),
					inputModel.MessageId);

				return this.Unauthorized();
			}
			catch (InvalidOperationException)
			{
				this.logger.LogWarning(
					LoggerMessages.UnsuccessfulReplyAddition,
					this.User.Id(),
					inputModel.MessageId);

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
		public async Task<IActionResult> MarkAsSeen([RegularExpression("^[0-9A-Fa-f]{24}$")] string messageId)
		{
			if (!this.ModelState.IsValid)
			{
				this.logger.LogWarning(
					LoggerMessages.MarkAsSeenWithInvalidInputData,
					this.User.Id());

				return this.BadRequest();
			}

			try
			{
				await this.messagesService.MarkMessageAsSeenAsync(messageId, this.User.Id(), this.User.IsAdmin());
			}
			catch (ArgumentException)
			{
				this.logger.LogWarning(
					LoggerMessages.UnsuccessfulMarkMessageAsSeen, 
					this.User.Id(), 
					messageId);

				return this.BadRequest();
			}

			return this.Ok();
		}
	}
}
