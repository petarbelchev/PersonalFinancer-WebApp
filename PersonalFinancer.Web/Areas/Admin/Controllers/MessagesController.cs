namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	using AutoMapper;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.SignalR;
	using PersonalFinancer.Services.Messages;
	using PersonalFinancer.Services.Users;
	using PersonalFinancer.Web.Hubs;
	using static PersonalFinancer.Common.Constants.RoleConstants;

	[Area("Admin")]
	[Authorize(Roles = AdminRoleName)]
	public class MessagesController : Web.Controllers.MessagesController
    {
        public MessagesController(
            IMessagesService messagesService,
            IUsersService usersService,
            IMapper mapper,
            IHubContext<AllMessagesHub> allMessagesHub,
            IHubContext<NotificationsHub> notificationsHub)
            : base(messagesService, usersService, mapper, allMessagesHub, notificationsHub)
        { }
    }
}
