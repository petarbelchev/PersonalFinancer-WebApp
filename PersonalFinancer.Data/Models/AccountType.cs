﻿namespace PersonalFinancer.Data.Models
{
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	
	using static Data.DataConstants.AccountType;

	public class AccountType
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(AccountTypeNameMaxLength,
			ErrorMessage = "Account type name max length must be {1} characters long.")]
		public string Name { get; set; } = null!;

		[ForeignKey(nameof(User))]
		public string? UserId { get; set; }
		public ApplicationUser? User { get; set; }
	}
}
