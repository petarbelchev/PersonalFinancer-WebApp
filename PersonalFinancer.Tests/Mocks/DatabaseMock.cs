using Microsoft.EntityFrameworkCore;
using PersonalFinancer.Data;

namespace PersonalFinancer.Tests.Mocks
{
	static class DatabaseMock
	{
		public static SqlDbContext Instance
		{
			get
			{
				var dbContextOptionsBuilder = new DbContextOptionsBuilder<SqlDbContext>()
					.UseInMemoryDatabase("PersonalFinancerInMemoryDb" + DateTime.Now.Ticks.ToString()).Options;

				return new SqlDbContext(dbContextOptionsBuilder, false);
			}
		}
	}
}
