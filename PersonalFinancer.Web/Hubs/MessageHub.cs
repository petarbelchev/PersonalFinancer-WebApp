namespace PersonalFinancer.Web.Hubs
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.SignalR;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Services.Messages.Models;

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
			await this.Clients.Group(messageId).SendAsync("ReceiveReply", reply);
			await this.Clients.Group(messageId).SendAsync("MarkAsSeen");

			return ResponseMessages.MessageSuccessfullySend;
		}
	}
}
