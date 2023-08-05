namespace PersonalFinancer.Web.Controllers
{
	using AutoMapper;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.SignalR;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Services.Messages;
	using PersonalFinancer.Services.Messages.Models;
	using PersonalFinancer.Services.Users;
	using PersonalFinancer.Web.CustomAttributes;
	using PersonalFinancer.Web.Hubs;
	using PersonalFinancer.Web.Models.Message;
	using System.ComponentModel.DataAnnotations;
	using static PersonalFinancer.Common.Constants.RoleConstants;
	using static PersonalFinancer.Common.Constants.HubConstants;

	[Authorize]
	public class MessagesController : Controller
	{
		private readonly IMessagesService messagesService;
		private readonly IUsersService usersService;
		private readonly IMapper mapper;
		private readonly IHubContext<AllMessagesHub> allMessagesHub;
		private readonly IHubContext<NotificationsHub> notificationsHub;
		private readonly ILogger<MessagesController> logger;

		public MessagesController(
			IMessagesService messagesService,
			IUsersService usersService,
			IMapper mapper,
			IHubContext<AllMessagesHub> allMessagesHub,
			IHubContext<NotificationsHub> notificationsHub,
			ILogger<MessagesController> logger)
		{
			this.messagesService = messagesService;
			this.usersService = usersService;
			this.mapper = mapper;
			this.allMessagesHub = allMessagesHub;
			this.notificationsHub = notificationsHub;
			this.logger = logger;
		}

		[HttpPost]
		[NoHtmlSanitizing]
		public async Task<IActionResult> Archive([RegularExpression("^[0-9A-Fa-f]{24}$")] string id)
		{
			if (!this.ModelState.IsValid)
			{
				this.logger.LogWarning(
					LoggerMessages.ArchiveMessageWithInvalidInputData,
					this.User.Id());

				return this.BadRequest();
			}

			try
			{
				await this.messagesService.ArchiveAsync(id, this.User.Id(), this.User.IsAdmin());
			}
			catch (UnauthorizedAccessException)
			{
				this.logger.LogWarning(
					LoggerMessages.UnauthorizedArchiveMessage,
					this.User.Id(),
					id);

				return this.Unauthorized();
			}
			catch (InvalidOperationException)
			{
				this.logger.LogWarning(
					LoggerMessages.UnsuccessfulMessageArchiving,
					this.User.Id(),
					id);

				return this.BadRequest();
			}

			return this.RedirectToAction(nameof(Index));
		}

		public IActionResult Archived() => this.View();

		[Authorize(Roles = UserRoleName)]
		public IActionResult Create() => this.View(new MessageModel());

		[Authorize(Roles = UserRoleName)]
		[HttpPost]
		public async Task<IActionResult> Create(MessageModel model)
		{
			if (!this.ModelState.IsValid)
				return this.View(model);

			var inputDTO = new MessageInputDTO
			{
				AuthorId = this.User.Id(),
				AuthorName = await this.usersService.UserFullNameAsync(this.User.IdToGuid()),
				Subject = model.Subject,
				Content = model.Content,
				Image = model.Image
			};

			string newMessageId;

			try
			{
				newMessageId = await this.messagesService.CreateAsync(inputDTO);
			}
			catch (ArgumentException ex)
			{
				this.ModelState.AddModelError(nameof(model.Image), ex.Message);

				return this.View(model);
			}

			IEnumerable<string> adminsIds = await this.usersService.GetAdminsIdsAsync();

			await this.notificationsHub.Clients
				.Users(adminsIds)
				.SendAsync(ReceiveNotificationMethodName);

			await this.allMessagesHub.Clients
				.Users(adminsIds)
				.SendAsync(RefreshMessagesMethodName);

			return this.RedirectToAction(nameof(Details), new { id = newMessageId });
		}

		[HttpPost]
		[NoHtmlSanitizing]
		public async Task<IActionResult> Delete([RegularExpression("^[0-9A-Fa-f]{24}$")] string id)
		{
			if (!this.ModelState.IsValid)
			{
				this.logger.LogWarning(
					LoggerMessages.DeleteMessageWithInvalidInputData,
					this.User.Id());

				return this.BadRequest();
			}

			IEnumerable<string> ids = this.User.IsAdmin()
				? new List<string> { await this.messagesService.GetMessageAuthorIdAsync(id) }
				: await this.usersService.GetAdminsIdsAsync();

			try
			{
				await this.messagesService.RemoveAsync(id, this.User.Id(), this.User.IsAdmin());
			}
			catch (UnauthorizedAccessException)
			{
				this.logger.LogWarning(
					LoggerMessages.UnauthorizedMessageDeletion,
					this.User.Id(),
					id);

				return this.Unauthorized();
			}
			catch (InvalidOperationException)
			{
				this.logger.LogWarning(
					LoggerMessages.UnsuccessfulMessageDeletion,
					this.User.Id(),
					id);

				return this.BadRequest();
			}

			await this.allMessagesHub.Clients
				.Users(ids)
				.SendAsync(RefreshMessagesMethodName);

			if (this.User.IsAdmin())
			{
				if (!await this.messagesService.HasUnseenMessagesByUserAsync(ids.First()))
				{
					await this.notificationsHub.Clients
						.Users(ids)
						.SendAsync(RemoveNotificationMethodName);
				}
			}
			else
			{
				if (!await this.messagesService.HasUnseenMessagesByAdminAsync())
				{
					await this.notificationsHub.Clients
						.Users(ids)
						.SendAsync(RemoveNotificationMethodName);
				}
			}

			return this.RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Details([RegularExpression("^[0-9A-Fa-f]{24}$")] string id)
		{
			if (!this.ModelState.IsValid)
			{
				this.logger.LogWarning(
					LoggerMessages.GetMessageDetailsWithInvalidInputData,
					this.User.Id());

				return this.BadRequest();
			}

			MessageDetailsDTO messageDTO;

			try
			{
				messageDTO = await this.messagesService
					.GetMessageAsync(id, this.User.Id(), this.User.IsAdmin());
			}
			catch (UnauthorizedAccessException)
			{
				this.logger.LogWarning(
					LoggerMessages.UnauthorizedGetMessageDetails,
					this.User.Id(),
					id);

				return this.Unauthorized();
			}
			catch (ArgumentException)
			{
				this.logger.LogWarning(
					LoggerMessages.UnsuccessfulMarkMessageAsSeen,
					this.User.Id(),
					id);

				return this.BadRequest();
			}
			catch (InvalidOperationException)
			{
				this.logger.LogWarning(
					LoggerMessages.GetMessageDetailsWithInvalidInputData,
					this.User.Id());

				return this.BadRequest();
			}

			var viewModel = this.mapper.Map<MessageDetailsViewModel>(messageDTO);

			if (messageDTO.Image?.Length > 0)
				viewModel.ImageToBase64String = Convert.ToBase64String(messageDTO.Image);

			return this.View(viewModel);
		}

		public IActionResult Index() => this.View();
	}
}
