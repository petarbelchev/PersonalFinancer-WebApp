namespace PersonalFinancer.Web.Hubs
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.SignalR;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Services.Users;
	using static PersonalFinancer.Common.Constants.HubConstants;

	[Authorize]
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

		public async Task<string> SendNotification(string authorId)
		{
			bool isUserAdmin = this.Context.User?.IsAdmin() ?? throw new InvalidOperationException();

			IEnumerable<string> ids = isUserAdmin
				? new List<string>() { authorId }
				: await this.usersService.GetAdminsIdsAsync();

			await this.Clients
				.Users(ids)
				.SendAsync(ReceiveNotificationMethodName);

			await this.allMessagesHub.Clients
				.Users(ids)
				.SendAsync(RefreshMessagesMethodName);

			return ResponseMessages.NotificationSuccessfullySend;
		}
	}
}
