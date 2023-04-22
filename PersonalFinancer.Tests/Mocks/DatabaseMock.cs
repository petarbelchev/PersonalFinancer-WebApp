namespace PersonalFinancer.Tests.Mocks
{
	using Microsoft.EntityFrameworkCore;

	using Data;

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
