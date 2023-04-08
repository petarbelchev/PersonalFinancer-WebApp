﻿namespace PersonalFinancer.Services.Accounts.Models
{
	using Services.Shared.Models;
	
	public class UserTransactionsOutputDTO
	{
		public string Id { get; set; } = null!;

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public IEnumerable<TransactionTableDTO> Transactions { get; set; } = null!;

		public int AllTransactionsCount { get; set; }
	}
}