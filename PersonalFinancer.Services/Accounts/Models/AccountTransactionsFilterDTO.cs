﻿namespace PersonalFinancer.Services.Accounts.Models
{
	public class AccountTransactionsFilterDTO
	{
		public Guid Id { get; set; } 
		
		public DateTime FromLocalTime { get; set; } 
		
		public DateTime ToLocalTime { get; set; }

		public int Page { get; set; } = 1;
	}
}
