namespace PersonalFinancer.Web.Hubs
{
	using Microsoft.AspNetCore.SignalR;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Services.Messages;
	using PersonalFinancer.Services.Messages.Models;
	using PersonalFinancer.Services.User;
	using PersonalFinancer.Web.Extensions;

	public class MessagesHub : Hub
	{
		private readonly IUsersService usersService;
		private readonly IMessagesService messagesService;

		public MessagesHub(
			IUsersService usersService,
			IMessagesService messagesService)
		{
			this.usersService = usersService;
			this.messagesService = messagesService;
		}

		public async Task JoinGroup(string messageId)
		{
			string groupName = messageId;
			await this.Groups.AddToGroupAsync(this.Context.ConnectionId, groupName);
		}

		public async Task SendMessage(string messageId, string replyContent)
		{
			string userId = this.Context.UserIdentifier ??
					throw new InvalidOperationException(string.Format(
						ExceptionMessages.NotNullableProperty, nameof(this.Context.User)));

			string userFullName = await this.usersService
				.UserFullNameAsync(Guid.Parse(userId));

			var dto = new ReplyInputDTO
			{
				MessageId = messageId,
				AuthorId = userId,
				AuthorName = userFullName,
				Content = replyContent,
				IsAuthorAdmin = this.Context.User?.IsAdmin() ?? false
			};

			ReplyOutputDTO? reply = await this.messagesService.AddReplyAsync(dto);

			if (reply != null)
			{
				await this.Clients.Group(messageId).SendAsync("ReceiveMessage", reply);
				await this.Clients.Group(messageId).SendAsync("MarkAsSeen");
			}
		}
	}
}
