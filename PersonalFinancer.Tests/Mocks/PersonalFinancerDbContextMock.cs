namespace PersonalFinancer.Tests.Mocks
{
    using Microsoft.EntityFrameworkCore;
    using PersonalFinancer.Data;

    static class PersonalFinancerDbContextMock
	{
		public static PersonalFinancerDbContext Instance
		{
			get
			{
				var dbContextOptionsBuilder = new DbContextOptionsBuilder<PersonalFinancerDbContext>()
					.UseInMemoryDatabase("PersonalFinancerInMemoryDb" + DateTime.Now.Ticks.ToString()).Options;

				return new PersonalFinancerDbContext(dbContextOptionsBuilder);
			}
		}
	}
}
