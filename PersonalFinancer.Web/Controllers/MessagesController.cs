namespace PersonalFinancer.Web.Controllers
{
	using AutoMapper;

	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Services.Messages;
	using Services.Messages.Models;
	using Services.User;

	using Web.Infrastructure;
	using Web.Models.Message;

	using static Data.Constants.RoleConstants;

	[Authorize]
	public class MessagesController : Controller
	{
		private readonly MessagesService messagesService;
		private readonly IUsersService usersService;
		private readonly IMapper mapper;

		public MessagesController(
			MessagesService messagesService,
			IUsersService usersService,
			IMapper mapper)
		{
			this.messagesService = messagesService;
			this.usersService = usersService;
			this.mapper = mapper;
		}

		public async Task<IActionResult> AllMessages()
		{
			var messages = User.IsAdmin()
				? await messagesService.GetAllAsync()
				: await messagesService.GetUserMessagesAsync(User.Id());

			return View(messages);
		}

		[Authorize(Roles = UserRoleName)]
		public IActionResult Create() => View(new MessageInputModel());

		[Authorize(Roles = UserRoleName)]
		[HttpPost]
		public async Task<IActionResult> Create(MessageInputModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			string newMessageId = await messagesService
				.CreateAsync(new MessageInputServiceModel
				{
					AuthorId = User.Id(),
					AuthorName = await usersService.FullName(User.Id()),
					Subject = model.Subject,
					Content = model.Content
				});

			return RedirectToAction(nameof(MessageDetails), new { id = newMessageId });
		}

		public async Task<IActionResult> Delete(string id)
		{
			try
			{
				await messagesService.RemoveAsync(id, User.Id(), User.IsAdmin());
			}
			catch (ArgumentException)
			{
				return Unauthorized();
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}

			return RedirectToAction(nameof(AllMessages));
		}

		public async Task<IActionResult> MessageDetails(string id)
		{
			try
			{
				MessageDetailsServiceModel message =
					await messagesService.GetMessageAsync(id, User.Id(), User.IsAdmin());

				var viewModel = mapper.Map<MessageDetailsViewModel>(message);

				return View(viewModel);
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> MessageDetails(ReplyInputModel inputModel)
		{
			if (!ModelState.IsValid)
			{
				MessageDetailsServiceModel message =
					await messagesService.GetMessageAsync(inputModel.Id, User.Id(), User.IsAdmin());

				var viewModel = mapper.Map<MessageDetailsViewModel>(message);
				viewModel.ReplyContent = inputModel.ReplyContent;

				return View(viewModel);
			}

			try
			{
				await messagesService.AddReplyAsync(new ReplyInputServiceModel
				{
					MessageId = inputModel.Id,
					AuthorId = User.Id(),
					AuthorName = await usersService.FullName(User.Id()),
					IsAuthorAdmin = User.IsAdmin(),
					Content = inputModel.ReplyContent
				});

				return RedirectToAction(nameof(MessageDetails), new { inputModel.Id });
			}
			catch (ArgumentException)
			{
				return Unauthorized();
			}
			catch (InvalidOperationException)
			{
				return BadRequest();
			}
		}
	}
}
