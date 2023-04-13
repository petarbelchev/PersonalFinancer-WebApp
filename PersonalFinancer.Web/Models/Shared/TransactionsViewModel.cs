﻿namespace PersonalFinancer.Web.Models.Shared
{
    using Services.Shared.Models;

    using Web.Models.Shared;

    using static Data.Constants;

    public class TransactionsViewModel
    {
        public IEnumerable<TransactionTableServiceModel> Transactions { get; set; } = null!;

        public PaginationModel Pagination { get; set; } = new PaginationModel
        {
            ElementsName = PaginationConstants.TransactionsName,
            ElementsPerPage = PaginationConstants.TransactionsPerPage
        };

        public string TransactionDetailsUrl { get; set; } = null!;
    }
}