using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinancer.Services.Messages;
using PersonalFinancer.Services.Messages.Models;
using PersonalFinancer.Services.User;
using PersonalFinancer.Web.Infrastructure.Extensions;
using PersonalFinancer.Web.Models.Message;
using static PersonalFinancer.Data.Constants.RoleConstants;

namespace PersonalFinancer.Web.Controllers
{
	[Authorize]
	public class MessagesController : Controller
	{
		private readonly IMessagesService messagesService;
		private readonly IUsersService usersService;
		private readonly IMapper mapper;

		public MessagesController(
			IMessagesService messagesService,
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

		[HttpPost]
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
			try
			{
				if (!ModelState.IsValid)
				{
					MessageDetailsServiceModel message =
						await messagesService.GetMessageAsync(inputModel.Id, User.Id(), User.IsAdmin());

					var viewModel = mapper.Map<MessageDetailsViewModel>(message);
					viewModel.ReplyContent = inputModel.ReplyContent;

					return View(viewModel);
				}

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
