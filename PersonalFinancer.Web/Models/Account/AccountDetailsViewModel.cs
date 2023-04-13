﻿namespace PersonalFinancer.Web.Models.Account
{
	using Services.Shared.Models;

	using Web.Models.Shared;
	
	using static Data.Constants;

	public class AccountDetailsViewModel
	{
		public string Id { get; set; } = null!;

		public string Name { get; set; } = null!;

		public decimal Balance { get; set; }

		public string CurrencyName { get; set; } = null!;

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public string OwnerId { get; set; } = null!;

		public IEnumerable<TransactionTableServiceModel> Transactions { get; set; }
			= new List<TransactionTableServiceModel>();

		public string ApiTransactionsEndpoint { get; set; }
			= HostConstants.ApiAccountTransactionsUrl;

		public RoutingModel Routing { get; set; } = new RoutingModel
		{
			Controller = "Accounts",
			Action = "AccountDetails"
		};

		public PaginationModel Pagination { get; set; } = new PaginationModel
		{
			ElementsPerPage = PaginationConstants.TransactionsPerPage,
			ElementsName = PaginationConstants.TransactionsName
		};
	}
}