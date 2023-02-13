namespace PersonalFinancer.Data.Configurations
{
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;
	
	using Enums;
	using Models;
	using static DataConstants.Category;

	internal class TransactionEntityTypeConfiguration : IEntityTypeConfiguration<Transaction>
	{
		public void Configure(EntityTypeBuilder<Transaction> builder)
		{
			builder.HasData(SeedTransactions());
		}

		private IEnumerable<Transaction> SeedTransactions()
		{
			var transactions = new List<Transaction>()
			{
				// Cash BGN
				new Transaction()
				{
					Id = Guid.NewGuid(),
					AccountId = Guid.Parse("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"),
					Amount = 200,
					CategoryId = Guid.Parse("e241b89f-b094-4f79-bb09-efc6f47c2cb3"),
					CreatedOn = DateTime.UtcNow.AddMonths(-3),
					Refference = CategoryInitialBalanceName,
					TransactionType = TransactionType.Income
				},
				new Transaction()
				{
					Id = Guid.NewGuid(),
					AccountId = Guid.Parse("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"),
					Amount = 5.65m,
					CategoryId = Guid.Parse("93cebd34-a9f5-4862-a8c9-3b6eea63e94c"),
					CreatedOn = DateTime.UtcNow.AddMonths(-2),
					Refference = "Lunch",
					TransactionType = TransactionType.Expense
				},
				new Transaction()
				{
					Id = Guid.NewGuid(),
					AccountId = Guid.Parse("ca5f67dd-78d7-4bb6-b42e-6a73dd79e805"),
					Amount = 4.80m,
					CategoryId = Guid.Parse("b58a7947-eecf-40d0-b84e-c6947fcbfd86"),
					CreatedOn = DateTime.UtcNow.AddMonths(-1),
					Refference = "Taxi",
					TransactionType = TransactionType.Expense
				},
				// Bank BGN
				new Transaction()
				{
					Id = Guid.NewGuid(),
					AccountId = Guid.Parse("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"),
					Amount = 1834.78m,
					CategoryId = Guid.Parse("e241b89f-b094-4f79-bb09-efc6f47c2cb3"),
					CreatedOn = DateTime.UtcNow.AddMonths(-3),
					Refference = CategoryInitialBalanceName,
					TransactionType = TransactionType.Income
				},
				new Transaction()
				{
					Id = Guid.NewGuid(),
					AccountId = Guid.Parse("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"),
					Amount = 100.00m,
					CategoryId = Guid.Parse("d59cbb57-3b9e-4b37-9b74-a375eecba8c8"),
					CreatedOn = DateTime.UtcNow.AddMonths(-3),
					Refference = "Electricity bill",
					TransactionType = TransactionType.Expense
				},
				new Transaction()
				{
					Id = Guid.NewGuid(),
					AccountId = Guid.Parse("ba7def5d-b00c-4e05-8d0b-5df2c47273b5"),
					Amount = 1000.00m,
					CategoryId = Guid.Parse("081a7be8-15c4-426e-872c-dfaf805e3fec"),
					CreatedOn = DateTime.UtcNow.AddMonths(-2),
					Refference = "Salary",
					TransactionType = TransactionType.Income
				},
				// Cash EUR
				new Transaction()
				{
					Id = Guid.NewGuid(),
					AccountId = Guid.Parse("70169197-5c32-4430-ab39-34c776533376"),
					Amount = 600,
					CategoryId = Guid.Parse("e241b89f-b094-4f79-bb09-efc6f47c2cb3"),
					CreatedOn = DateTime.UtcNow.AddMonths(-3),
					Refference = CategoryInitialBalanceName,
					TransactionType = TransactionType.Income
				},
				new Transaction()
				{
					Id = Guid.NewGuid(),
					AccountId = Guid.Parse("70169197-5c32-4430-ab39-34c776533376"),
					Amount = 24.29m,
					CategoryId = Guid.Parse("96e441e3-c5a6-427f-bb32-85940242d9ee"),
					CreatedOn = DateTime.UtcNow.AddMonths(-2),
					Refference = "Health Insurance",
					TransactionType = TransactionType.Expense
				},
				new Transaction()
				{
					Id = Guid.NewGuid(),
					AccountId = Guid.Parse("70169197-5c32-4430-ab39-34c776533376"),
					Amount = 250m,
					CategoryId = Guid.Parse("081a7be8-15c4-426e-872c-dfaf805e3fec"),
					CreatedOn = DateTime.UtcNow,
					Refference = "Salary",
					TransactionType = TransactionType.Income
				},
				// Bank EUR
				new Transaction()
				{
					Id = Guid.NewGuid(),
					AccountId = Guid.Parse("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"),
					Amount = 200,
					CategoryId = Guid.Parse("e241b89f-b094-4f79-bb09-efc6f47c2cb3"),
					CreatedOn = DateTime.UtcNow.AddMonths(-3),
					Refference = CategoryInitialBalanceName,
					TransactionType = TransactionType.Income
				},
				new Transaction()
				{
					Id = Guid.NewGuid(),
					AccountId = Guid.Parse("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"),
					Amount = 750m,
					CategoryId = Guid.Parse("081a7be8-15c4-426e-872c-dfaf805e3fec"),
					CreatedOn = DateTime.UtcNow.AddMonths(-2),
					Refference = "Salary",
					TransactionType = TransactionType.Income
				},
				new Transaction()
				{
					Id = Guid.NewGuid(),
					AccountId = Guid.Parse("44c67e3a-2dfe-491c-b7fc-eb78fe8b8946"),
					Amount = 49.99m,
					CategoryId = Guid.Parse("b58a7947-eecf-40d0-b84e-c6947fcbfd86"),
					CreatedOn = DateTime.UtcNow.AddMonths(-2),
					Refference = "Flight ticket",
					TransactionType = TransactionType.Expense
				},
				// Bank USD
				new Transaction()
				{
					Id = Guid.NewGuid(),
					AccountId = Guid.Parse("303430dc-63a3-4436-8907-a274ec29f608"),
					Amount = 1487.23m,
					CategoryId = Guid.Parse("e241b89f-b094-4f79-bb09-efc6f47c2cb3"),
					CreatedOn = DateTime.UtcNow.AddMonths(-8),
					Refference = CategoryInitialBalanceName,
					TransactionType = TransactionType.Income
				}
			};

			return transactions;
		}
	}
}
