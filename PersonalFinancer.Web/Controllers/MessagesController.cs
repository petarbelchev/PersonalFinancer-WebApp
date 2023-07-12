namespace PersonalFinancer.Web.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.SignalR;
	using PersonalFinancer.Services.Messages;
    using PersonalFinancer.Services.Messages.Models;
    using PersonalFinancer.Services.User;
	using PersonalFinancer.Web.CustomAttributes;
	using PersonalFinancer.Web.Extensions;
	using PersonalFinancer.Web.Hubs;
	using PersonalFinancer.Web.Models.Message;
	using System.ComponentModel.DataAnnotations;
	using static PersonalFinancer.Common.Constants.RoleConstants;

    [Authorize]
    public class MessagesController : Controller
    {
        private readonly IMessagesService messagesService;
        private readonly IUsersService usersService;
        private readonly IMapper mapper;
        private readonly IHubContext<AllMessagesHub> allMessagesHub;
        private readonly IHubContext<NotificationsHub> notificationsHub;

		public MessagesController(
            IMessagesService messagesService,
            IUsersService usersService,
            IMapper mapper,
            IHubContext<AllMessagesHub> allMessagesHub,
            IHubContext<NotificationsHub> notificationsHub)
        {
            this.messagesService = messagesService;
            this.usersService = usersService;
            this.mapper = mapper;
            this.allMessagesHub = allMessagesHub;
            this.notificationsHub = notificationsHub;
        }

        public async Task<IActionResult> All()
        {
            MessagesDTO messagesDTO = this.User.IsAdmin()
                ? await this.messagesService.GetAllMessagesAsync()
                : await this.messagesService.GetUserMessagesAsync(this.User.Id());

            return this.View(new MessagesViewModel(messagesDTO));
        }

        [HttpPost]
        [NotRequireHtmlEncoding]
        public async Task<IActionResult> Archive([Required] string id)
        {
            try
            {
                await this.messagesService.ArchiveAsync(id, this.User.Id(), this.User.IsAdmin());
            }
            catch (ArgumentException)
            {
                return this.Unauthorized();
            }
            catch (InvalidOperationException) 
            {
                return this.BadRequest();
            }

            return this.RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> Archived()
		{
			MessagesDTO messagesDTO = this.User.IsAdmin()
				? await this.messagesService.GetAllArchivedAsync()
				: await this.messagesService.GetUserArchivedAsync(this.User.Id());

			return this.View(new MessagesViewModel(messagesDTO));
		}

        [Authorize(Roles = UserRoleName)]
        public IActionResult Create() => this.View(new MessageInputModel());

        [Authorize(Roles = UserRoleName)]
        [HttpPost]
        public async Task<IActionResult> Create(MessageInputModel model)
        {
            if (!this.ModelState.IsValid)
                return this.View(model);

            MessageOutputDTO messageDTO = await this.messagesService
                .CreateAsync(new MessageInputDTO
                {
                    AuthorId = this.User.Id(),
                    AuthorName = await this.usersService.UserFullNameAsync(this.User.IdToGuid()),
                    Subject = model.Subject,
                    Content = model.Content
                });

			IEnumerable<string> adminsIds = await this.usersService.GetAdminsIdsAsync();

			await this.notificationsHub.Clients
				.Users(adminsIds)
				.SendAsync("ReceiveNotification");

			await this.allMessagesHub.Clients
                .Users(adminsIds)
                .SendAsync("RefreshMessages");

            return this.RedirectToAction(nameof(Details), new { id = messageDTO.Id });
        }

        [HttpPost]
        [NotRequireHtmlEncoding]
        public async Task<IActionResult> Delete([Required] string id)
		{
			IEnumerable<string> ids = this.User.IsAdmin()
				? new List<string> { await this.messagesService.GetMessageAuthorIdAsync(id) }
				: await this.usersService.GetAdminsIdsAsync();

			try
			{
                await this.messagesService.RemoveAsync(id, this.User.Id(), this.User.IsAdmin());
            }
            catch (ArgumentException)
            {
                return this.Unauthorized();
            }
            catch (InvalidOperationException)
            {
                return this.BadRequest();
            }

			await this.allMessagesHub.Clients
				.Users(ids)
				.SendAsync("RefreshMessages");

            if (this.User.IsAdmin())
            {
                if (!await this.messagesService.HasUnseenMessagesByUserAsync(ids.First()))
                {
                    await this.notificationsHub.Clients
                        .Users(ids)
                        .SendAsync("RemoveNotification");
                }
            }
            else
			{
				if (!await this.messagesService.HasUnseenMessagesByAdminAsync())
				{
					await this.notificationsHub.Clients
						.Users(ids)
						.SendAsync("RemoveNotification");
				}
			}

			return this.RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> Details([Required] string id)
        {
            try
            {
                MessageDetailsDTO message =
                    await this.messagesService.GetMessageAsync(id, this.User.Id(), this.User.IsAdmin());

                MessageDetailsViewModel viewModel = this.mapper.Map<MessageDetailsViewModel>(message);

                return this.View(viewModel);
            }
            catch (InvalidOperationException)
            {
                return this.BadRequest();
            }
        }
    }
}
