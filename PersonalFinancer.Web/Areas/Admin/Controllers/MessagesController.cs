namespace PersonalFinancer.Web.Areas.Admin.Controllers
{
	using AutoMapper;

	using Microsoft.AspNetCore.Mvc;

	using Services.Messages;
	using Services.User;

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
