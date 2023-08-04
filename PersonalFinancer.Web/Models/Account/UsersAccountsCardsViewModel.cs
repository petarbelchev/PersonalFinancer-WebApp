namespace PersonalFinancer.Web.Models.Account
{
	using PersonalFinancer.Services.Accounts.Models;
	using PersonalFinancer.Services.Shared.Models;
	using PersonalFinancer.Web.Models.Shared;
    using static PersonalFinancer.Common.Constants.PaginationConstants;

    public class UsersAccountsCardsViewModel
	{
        public UsersAccountsCardsViewModel(
			AccountsCardsDTO usersCardsData,
			int page)
        {
			this.AccountsCards = usersCardsData.Accounts;

			this.Pagination = new PaginationModel(
				AccountsName, 
				AccountsPerPage, 
				usersCardsData.TotalAccountsCount,
				page);
		}

        public IEnumerable<AccountCardDTO> AccountsCards { get; set; }

		public PaginationModel Pagination { get; private set; }
	}
}
