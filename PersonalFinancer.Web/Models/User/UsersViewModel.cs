namespace PersonalFinancer.Web.Models.User
{
    using PersonalFinancer.Services.User.Models;
    using PersonalFinancer.Web.Models.Shared;
    using static PersonalFinancer.Services.Constants;
    using static PersonalFinancer.Web.Constants;

    public class UsersViewModel
	{
        public UsersViewModel(UsersInfoDTO usersInfoDTO, int page)
        {
			this.Users = usersInfoDTO.Users;

			this.Pagination = new PaginationModel(
				PaginationConstants.UsersName,
				PaginationConstants.UsersPerPage,
				usersInfoDTO.TotalUsersCount,
				page);

			this.ApiUsersEndpoint = UrlPathConstants.ApiUsersEndpoint;

			this.Routing = new RoutingModel
			{
				Area = "Admin",
				Controller = "Users",
				Action = "Index"
			};
		}

        public IEnumerable<UserInfoDTO> Users { get; private set; }

		public PaginationModel Pagination { get; private set; }

		public string ApiUsersEndpoint { get; private set; }

		public RoutingModel Routing { get; private set; }
	}
}
