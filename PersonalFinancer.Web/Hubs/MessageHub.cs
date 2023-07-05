namespace PersonalFinancer.Web.Hubs
{
	using Microsoft.AspNetCore.SignalR;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Services.Messages;
	using PersonalFinancer.Services.Messages.Models;
	using PersonalFinancer.Services.User;
	using PersonalFinancer.Web.Extensions;

	public class MessageHub : Hub
	{
		private readonly IUsersService usersService;
		private readonly IMessagesService messagesService;

		public MessageHub(
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

		public async Task<string> SendMessage(string messageId, string replyContent)
		{
			string userId = this.Context.UserIdentifier ?? throw new InvalidOperationException();
			string userFullName = await this.usersService.UserFullNameAsync(Guid.Parse(userId));

			var dto = new ReplyInputDTO
			{
				MessageId = messageId,
				AuthorId = userId,
				AuthorName = userFullName,
				Content = replyContent,
				IsAuthorAdmin = this.Context.User?.IsAdmin() ?? throw new InvalidOperationException()
			};

			ReplyOutputDTO? reply;

			try
			{
				reply = await this.messagesService.AddReplyAsync(dto);
			}
			catch (ArgumentException ex)
			{
				return ex.Message;
			}

			if (reply == null)
				throw new InvalidOperationException();

			await this.Clients.Group(messageId).SendAsync("ReceiveMessage", reply);
			await this.Clients.Group(messageId).SendAsync("MarkAsSeen");

			return ResponseMessages.MessageSuccessfullySent;
		}
	}
}
