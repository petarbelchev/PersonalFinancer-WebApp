namespace PersonalFinancer.Web.Models.User
{
	using PersonalFinancer.Services.Users.Models;
	using PersonalFinancer.Web.Models.Shared;
	using static PersonalFinancer.Common.Constants.PaginationConstants;

	public class UsersViewModel
	{
		public UsersViewModel(UsersInfoDTO usersInfoDTO, int page)
		{
			this.Users = usersInfoDTO.Users;

			this.Pagination = new PaginationModel(
				UsersName, 
				UsersPerPage, 
				usersInfoDTO.TotalUsersCount, 
				page);
		}

		public IEnumerable<UserInfoDTO> Users { get; private set; }

		public PaginationModel Pagination { get; private set; }
	}
}
