using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinancer.Data.Models;

namespace PersonalFinancer.Data.Configurations
{
	internal class TransactionEntityTypeConfiguration : IEntityTypeConfiguration<Transaction>
	{
		public void Configure(EntityTypeBuilder<Transaction> builder)
		{
			builder.HasData(new Transaction[]
			{
				new Transaction
				{
					Id = 1,
					AccountId = 1,
					Amount = 25,
					CategoryId = 3,
					Refference = "My first transport transaction."
				}
			});
		}
	}
}
