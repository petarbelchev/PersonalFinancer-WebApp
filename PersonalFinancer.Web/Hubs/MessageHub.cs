namespace PersonalFinancer.Web.Hubs
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.SignalR;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Services.Messages.Models;
	using static PersonalFinancer.Common.Constants.HubConstants;

	[Authorize]
	public class MessageHub : Hub
	{
		public async Task JoinGroup(string messageId)
		{
			string groupName = messageId;
			await this.Groups.AddToGroupAsync(this.Context.ConnectionId, groupName);
		}

		public async Task<string> SendReply(string messageId, ReplyOutputDTO reply)
		{
			await this.Clients
				.Group(messageId)
				.SendAsync(ReceiveReplyMethodName, reply);

			await this.Clients
				.Group(messageId)
				.SendAsync(MarkAsSeenMethodName);

			return ResponseMessages.MessageSuccessfullySend;
		}
	}
}
