namespace PersonalFinancer.Web.Models.User
{
	using PersonalFinancer.Services.User.Models;
	using PersonalFinancer.Web.Models.Shared;
	using static PersonalFinancer.Common.Constants.PaginationConstants;

	public class UsersViewModel
	{
		public UsersViewModel(UsersInfoDTO usersInfoDTO, int page)
		{
			this.Users = usersInfoDTO.Users;

			this.Pagination = new PaginationModel(
				UsersName, UsersPerPage, usersInfoDTO.TotalUsersCount, page);

			this.Routing = new RoutingModel
			{
				Area = "Admin",
				Controller = "Users",
				Action = "Index"
			};
		}

		public IEnumerable<UserInfoDTO> Users { get; private set; }

		public PaginationModel Pagination { get; private set; }

		public RoutingModel Routing { get; private set; }
	}
}
