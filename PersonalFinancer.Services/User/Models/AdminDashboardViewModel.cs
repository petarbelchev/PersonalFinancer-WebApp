namespace PersonalFinancer.Services.User.Models
{
	public class AdminDashboardViewModel
	{
        public string AdminFullName { get; set; } = null!;

        public int RegisteredUsers { get; set; }

		public int CreatedAccounts { get; set; }
	}
}
