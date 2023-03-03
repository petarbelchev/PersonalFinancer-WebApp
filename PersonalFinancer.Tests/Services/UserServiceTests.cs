namespace PersonalFinancer.Tests.Services
{
	using NUnit.Framework;

	using PersonalFinancer.Services.Accounts;
	using PersonalFinancer.Services.Category;
	using PersonalFinancer.Services.Transactions;
	using PersonalFinancer.Services.User;

	[TestFixture]
	class UserServiceTests : UnitTestsBase
	{
		private IUserService userService;
		private IAccountService accountService;
		private ITransactionsService transactionsService;
		private ICategoryService categoryService;

		[SetUp] 
		public void SetUp()
		{
			this.categoryService = new CategoryService(this.data, this.mapper);
			this.transactionsService = new TransactionsService(this.data, this.mapper);
			this.accountService = new AccountService(this.data, this.mapper, this.transactionsService, this.categoryService);
			this.userService = new UserService(this.data, this.accountService, this.transactionsService, this.mapper);
		}

		[Test]
		public async Task FullName_ShouldReturnUsersFullName_WithValidId()
		{
			//Arrange
			string expectedFullName = $"{this.User1.FirstName} {this.User1.LastName}";

			//Act
			string actualFullName = await userService.FullName(this.User1.Id);

			//Assert
			Assert.That(actualFullName, Is.EqualTo(expectedFullName));
		}

		[Test]
		public void FullName_ShouldThrowException_WithInvalidId()
		{
			//Act & Assert
			Assert.That(async () => await userService.FullName(Guid.NewGuid().ToString()),
				Throws.TypeOf<ArgumentNullException>().With.Property("ParamName")
				.EqualTo("User does not exist."));
		}
	}
}
