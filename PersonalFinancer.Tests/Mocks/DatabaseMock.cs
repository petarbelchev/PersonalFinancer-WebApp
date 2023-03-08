using Microsoft.EntityFrameworkCore;

using PersonalFinancer.Data;

namespace PersonalFinancer.Tests.Mocks
{
	static class DatabaseMock
	{
		public static PersonalFinancerDbContext Instance
		{
			get
			{
				var dbContextOptionsBuilder = new DbContextOptionsBuilder<PersonalFinancerDbContext>()
					.UseInMemoryDatabase("PersonalFinancerInMemoryDb" + DateTime.Now.Ticks.ToString()).Options;

				return new PersonalFinancerDbContext(dbContextOptionsBuilder, false);
			}
		}
	}
}
