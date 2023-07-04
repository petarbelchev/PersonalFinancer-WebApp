namespace PersonalFinancer.Web.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.SignalR;
	using PersonalFinancer.Services.Messages;
    using PersonalFinancer.Services.Messages.Models;
    using PersonalFinancer.Services.User;
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

        public async Task<IActionResult> AllMessages()
        {
            IEnumerable<MessageOutputDTO> messages = this.User.IsAdmin()
                ? await this.messagesService.GetAllAsync()
                : await this.messagesService.GetUserMessagesAsync(this.User.Id());

            return this.View(messages);
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

			IEnumerable<string> adminsIds = await this.usersService.GetAdminsIds();

			await this.notificationsHub.Clients
				.Users(adminsIds)
				.SendAsync("ReceiveNotification");

			await this.allMessagesHub.Clients
                .Users(adminsIds)
                .SendAsync("ReceiveNotification", messageDTO.Id, messageDTO.Subject, messageDTO.CreatedOn);

            return this.RedirectToAction(nameof(MessageDetails), new { id = messageDTO.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([Required] string id)
        {
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

            // TODO: When admin delete user message...
            IEnumerable<string> adminsIds = /*this.User.IsAdmin()
                ? new List<string> { await this.messagesService.GetMessageAuthorIdAsync(id) }
                : */await this.usersService.GetAdminsIds();

			await this.allMessagesHub.Clients
				.Users(adminsIds)
				.SendAsync("DeleteMessage", id);

			return this.RedirectToAction(nameof(AllMessages));
        }

        public async Task<IActionResult> MessageDetails([Required] string id)
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

        [HttpPost]
        public async Task<IActionResult> MessageDetails(ReplyInputModel inputModel)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    MessageDetailsDTO message =
                        await this.messagesService.GetMessageAsync(inputModel.Id, this.User.Id(), this.User.IsAdmin());

                    MessageDetailsViewModel viewModel = this.mapper.Map<MessageDetailsViewModel>(message);
                    viewModel.ReplyContent = inputModel.ReplyContent;

                    return this.View(viewModel);
                }

                await this.messagesService.AddReplyAsync(new ReplyInputDTO
                {
                    MessageId = inputModel.Id,
                    AuthorId = this.User.Id(),
                    AuthorName = await this.usersService.UserFullNameAsync(this.User.IdToGuid()),
                    IsAuthorAdmin = this.User.IsAdmin(),
                    Content = inputModel.ReplyContent
                });

                return this.RedirectToAction(nameof(MessageDetails), new { inputModel.Id });
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
    }
}
