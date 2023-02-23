﻿namespace PersonalFinancer.Services.Account.Models
{
    public class TransactionExtendedViewModel
    {
        public Guid Id { get; set; }

        public string? AccountName { get; init; }

        public string AccountCurrencyName { get; init; } = null!;

        public decimal Amount { get; init; }

        public string CategoryName { get; init; } = null!;

        public string TransactionType { get; init; } = null!;

        public DateTime CreatedOn { get; init; }

        public string Refference { get; init; } = null!;
    }
}
