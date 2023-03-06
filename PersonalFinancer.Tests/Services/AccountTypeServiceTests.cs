using NUnit.Framework;

using PersonalFinancer.Services.AccountTypes;
using PersonalFinancer.Services.AccountTypes.Models;

namespace PersonalFinancer.Tests.Services
{
	internal class AccountTypeServiceTests : UnitTestsBase
	{
		private IAccountTypeService accountTypeService;

		[SetUp]
		public void SetUp()
		{
			this.accountTypeService = new AccountTypeService(this.data, this.mapper);
		}

		[Test]
		public async Task AccountTypesViewModel_ShouldReturnCorrectData_WithValidUserId()
		{
			//Arrange
			IEnumerable<AccountTypeViewModel> accountTypesInDb = data.AccountTypes
				.Where(a => (a.UserId == null || a.UserId == this.User1.Id) && !a.IsDeleted)
			.Select(a => mapper.Map<AccountTypeViewModel>(a))
				.AsEnumerable();

			//Act
			IEnumerable<AccountTypeViewModel> actualAccountTypes = await accountTypeService
				.AccountTypesViewModel(this.User1.Id);

			//Assert
			Assert.That(actualAccountTypes, Is.Not.Null);
			Assert.That(actualAccountTypes.Count(), Is.EqualTo(accountTypesInDb.Count()));
			for (int i = 0; i < actualAccountTypes.Count(); i++)
			{
				Assert.That(actualAccountTypes.ElementAt(i).Id,
					Is.EqualTo(accountTypesInDb.ElementAt(i).Id));
				Assert.That(actualAccountTypes.ElementAt(i).Name,
					Is.EqualTo(accountTypesInDb.ElementAt(i).Name));
			}
		}
	}
}
