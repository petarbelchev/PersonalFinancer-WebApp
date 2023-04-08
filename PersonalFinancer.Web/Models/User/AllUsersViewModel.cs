namespace PersonalFinancer.Web.Models.User
{
	using Web.Models.Shared;

	using static Data.Constants.HostConstants;

	public class AllUsersViewModel
	{
		public IEnumerable<UserViewModel> Users { get; set; } = null!;

		public PaginationModel Pagination { get; set; } = new PaginationModel
		{
			ElementsName = "users",
		};

		public string ApiUsersEndpoint { get; set; }
			= ApiUsersUrl;

		public RoutingModel Routing { get; set; } = new RoutingModel
		{
			Area = "Admin",
			Controller = "Users",
			Action = "Index"
		};
	}
}
