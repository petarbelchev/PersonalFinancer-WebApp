using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PersonalFinancer.Services.Messages;
using PersonalFinancer.Services.User;

namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class MessagesController : Web.Controllers.MessagesController
	{
		public MessagesController(
			IMessagesService messagesService,
			IUsersService usersService,
			IMapper mapper)
			: base(messagesService, usersService, mapper)
		{ }
	}
}
