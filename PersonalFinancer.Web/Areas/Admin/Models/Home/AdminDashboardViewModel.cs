namespace PersonalFinancer.Web.Areas.Admin.Models.Home
{
    public class AdminDashboardViewModel
    {
        public string AdminFullName { get; set; } = null!;

        public int RegisteredUsers { get; set; }

        public int CreatedAccounts { get; set; }

        public string AccountsCashFlowEndpoint { get; set; } = null!;
    }
}
