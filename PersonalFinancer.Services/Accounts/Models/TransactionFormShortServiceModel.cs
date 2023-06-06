﻿using PersonalFinancer.Data.Enums;

namespace PersonalFinancer.Services.Accounts.Models
{
	public class TransactionFormShortServiceModel
	{
		public decimal Amount { get; set; }

		public string OwnerId { get; set; } = null!;

		public string CategoryId { get; set; } = null!;

		public string AccountId { get; set; } = null!;

		public DateTime CreatedOn { get; set; }

		public string Refference { get; set; } = null!;

		public TransactionType TransactionType { get; set; }

		public bool IsInitialBalance { get; set; }
	}
}
