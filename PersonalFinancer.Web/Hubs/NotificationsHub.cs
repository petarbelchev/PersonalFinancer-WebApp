namespace PersonalFinancer.Web.Hubs
{
	using Microsoft.AspNetCore.SignalR;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Services.User;
	using PersonalFinancer.Web.Extensions;

	public class NotificationsHub : Hub
	{
		private readonly IUsersService usersService;
		private readonly IHubContext<AllMessagesHub> allMessagesHub;

		public NotificationsHub(
			IUsersService usersService,
			IHubContext<AllMessagesHub> allMessagesHub)
		{
			this.usersService = usersService;
			this.allMessagesHub = allMessagesHub;
		}

		public async Task<string> SendNotification(string authorId, string messageId)
		{
			bool isUserAdmin = this.Context.User?.IsAdmin() ?? throw new InvalidOperationException();

			IEnumerable<string> ids = isUserAdmin
				? new List<string>() { authorId }
				: await this.usersService.GetAdminsIdsAsync();

			await this.Clients.Users(ids).SendAsync("ReceiveNotification");
			await this.allMessagesHub.Clients.Users(ids).SendAsync("ReceiveNotification", messageId);

			return ResponseMessages.NotificationSuccessfullySent;
		}
	}
}
