using PersonalFinancer.Services.Shared.Models;

namespace PersonalFinancer.Services.User.Models
{
	public class AllUsersViewModel
	{
		public IEnumerable<UserViewModel> Users { get; set; } = null!;

        public PaginationModel Pagination { get; set; } = new PaginationModel();
    }
}
