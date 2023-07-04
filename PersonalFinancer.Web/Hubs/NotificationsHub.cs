namespace PersonalFinancer.Web.Hubs
{
	using Microsoft.AspNetCore.SignalR;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Services.User;
	using PersonalFinancer.Web.Extensions;

	public class NotificationsHub : Hub
	{
		private readonly IUsersService usersService;

		public NotificationsHub(IUsersService usersService) 
			=> this.usersService = usersService;

		public async Task SendNotification(string authorId)
		{
			bool isUserAdmin = this.Context.User?.IsAdmin() ?? 
				throw new InvalidOperationException(string.Format(
					ExceptionMessages.NotNullableProperty, nameof(this.Context.User)));

			IEnumerable<string> ids = isUserAdmin
				? new List<string>() { authorId }
				: await this.usersService.GetAdminsIds();

			await this.Clients.Users(ids).SendAsync("ReceiveNotification");
		}
	}
}
