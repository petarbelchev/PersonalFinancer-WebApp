namespace PersonalFinancer.Tests.Mocks
{
	using Microsoft.EntityFrameworkCore;

	using Data;

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
