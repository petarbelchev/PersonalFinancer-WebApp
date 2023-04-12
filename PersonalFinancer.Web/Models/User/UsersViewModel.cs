namespace PersonalFinancer.Services.User.Models
{
	using Web.Models.Shared;

	using static Data.Constants;

	public class UsersViewModel
	{
		public IEnumerable<UserServiceModel> Users { get; set; } = null!;

		public PaginationModel Pagination { get; set; } = new PaginationModel
		{
			ElementsName = PaginationConstants.UsersName,
			ElementsPerPage = PaginationConstants.UsersPerPage 
		};

		public string ApiUsersEndpoint { get; set; }
			= HostConstants.ApiUsersUrl;

		public RoutingModel Routing { get; set; } = new RoutingModel
		{
			Area = "Admin",
			Controller = "Users",
			Action = "Index"
		};
	}
}
