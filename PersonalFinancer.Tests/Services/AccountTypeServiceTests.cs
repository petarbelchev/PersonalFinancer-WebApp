using NUnit.Framework;

using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.AccountTypes;
using PersonalFinancer.Services.AccountTypes.Models;

namespace PersonalFinancer.Tests.Services
{
	[TestFixture]
	internal class AccountTypeServiceTests : UnitTestsBase
	{
		private IAccountTypeService accountTypeService;

		[SetUp]
		public void SetUp()
		{
			this.accountTypeService = new AccountTypeService(this.data, this.mapper, this.memoryCache);
		}

		[Test]
		public async Task AccountTypesViewModel_ShouldReturnCorrectData_WithValidUserId()
		{
			//Arrange
			IEnumerable<AccountTypeViewModel> accountTypesInDb = data.AccountTypes
				.Where(a => a.OwnerId == this.User1.Id && !a.IsDeleted)
				.OrderBy(a => a.Name)
				.Select(a => mapper.Map<AccountTypeViewModel>(a))
				.AsEnumerable();

			//Act
			IEnumerable<AccountTypeViewModel> actualAccountTypes = await accountTypeService
				.GetUserAccountTypesViewModel(this.User1.Id);

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

		[Test]
		public async Task CreateAccountType_ShouldAddNewAccountType_WithValidParams()
		{
			//Arrange
			var newAccountTypeName = "NewAccountType";
			int countBefore = data.AccountTypes.Count();

			//Act
			AccountTypeViewModel newCategory = await accountTypeService
				.CreateAccountType(this.User1.Id, newAccountTypeName);

			int countAfter = data.AccountTypes.Count();

			//Assert
			Assert.That(countAfter, Is.EqualTo(countBefore + 1));
			Assert.That(newCategory.Id, Is.Not.Null);
			Assert.That(newCategory.Name, Is.EqualTo(newAccountTypeName));
		}

		[Test]
		public async Task CreateAccountType_ShouldRecreateDeletedBeforeAccountType_WithValidParams()
		{
			//Arrange
			var deletedAccType = new AccountType
			{
				Name = "DeletedAccType",
				OwnerId = this.User1.Id,
				IsDeleted = true
			};
			data.AccountTypes.Add(deletedAccType);
			await data.SaveChangesAsync();
			int countBefore = data.AccountTypes.Count();

			//Assert
			Assert.That(async () =>
			{
				var deletedAcc = await data.AccountTypes.FindAsync(deletedAccType.Id);
				Assert.That(deletedAcc, Is.Not.Null);
				return deletedAcc.IsDeleted;
			}, Is.True);

			//Act
			AccountTypeViewModel newAccountType = await accountTypeService
				.CreateAccountType(this.User1.Id, deletedAccType.Name);

			int countAfter = data.AccountTypes.Count();

			//Assert
			Assert.That(countAfter, Is.EqualTo(countBefore));
			Assert.That(newAccountType.Id, Is.Not.Null);
			Assert.That(newAccountType.Name, Is.EqualTo(deletedAccType.Name));
		}

		[Test]
		public async Task CreateAccountType_ShouldAddNewAccTypeWhenAnotherUserHaveTheSameAccType()
		{
			//Arrange
			var user2AccType = new AccountType 
			{ 
				Name = "User2AccType", 
				OwnerId = this.User2.Id 
			};

			data.AccountTypes.Add(user2AccType);
			await data.SaveChangesAsync();
			int countBefore = data.AccountTypes.Count();

			//Assert
			Assert.That(await data.AccountTypes.FindAsync(user2AccType.Id), Is.Not.Null);

			//Act
			AccountTypeViewModel newAccountType = await accountTypeService
				.CreateAccountType(this.User1.Id, user2AccType.Name);

			int countAfter = data.AccountTypes.Count();

			//Assert
			Assert.That(countAfter, Is.EqualTo(countBefore + 1));
			Assert.That(newAccountType.Id, Is.Not.Null);
			Assert.That(newAccountType.Name, Is.EqualTo("User2AccType"));
		}

		[Test]
		public void CreateAccountType_ShouldThrowException_WhenAccTypeExist()
		{
			//Arrange

			//Act & Assert
			Assert.That(async () => await accountTypeService.CreateAccountType(this.User1.Id, this.AccountType1.Name),
				Throws.TypeOf<ArgumentException>().With.Message.EqualTo("Account Type with the same name exist!"));
		}

		[Test]
		[TestCase("A")]
		[TestCase("NameWith16Chars!")]
		public void CreateAccountType_ShouldThrowException_WithInvalidName(string accountTypeName)
		{
			//Arrange

			//Act & Assert
			Assert.That(async () => await accountTypeService.CreateAccountType(this.User1.Id, accountTypeName),
				Throws.TypeOf<ArgumentException>().With.Message
					.EqualTo("Account Type name must be between 2 and 15 characters long."));
		}

		[Test]
		public async Task DeleteAccountType_ShouldRemoveAccType_WithValidParams()
		{
			//Arrange
			var newAccType = new AccountType() { Name = "NewAccType", OwnerId = this.User1.Id };
			data.AccountTypes.Add(newAccType);
			await data.SaveChangesAsync();

			//Assert
			Assert.That(await data.AccountTypes.FindAsync(newAccType.Id), Is.Not.Null);
			Assert.That(newAccType.IsDeleted, Is.False);

			//Act
			await accountTypeService.DeleteAccountType(newAccType.Id, this.User1.Id);

			//Assert
			Assert.That(newAccType.IsDeleted, Is.True);
		}

		[Test]
		public void DeleteAccountType_ShouldThrowException_WhenAccTypeNotExist()
		{
			//Act & Assert
			Assert.That(async () => await accountTypeService.DeleteAccountType(Guid.NewGuid().ToString(), this.User1.Id),
				Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task DeleteAccountType_ShouldThrowException_WhenUserIsNotOwner()
		{
			//Arrange
			var user2AccType = new AccountType() { Name = "ForDelete", OwnerId = this.User2.Id };
			data.AccountTypes.Add(user2AccType);
			await data.SaveChangesAsync();

			//Act & Assert
			Assert.That(async () => await accountTypeService
					.DeleteAccountType(user2AccType.Id, this.User1.Id),
				Throws.TypeOf<InvalidOperationException>()
					.With.Message.EqualTo("You can't delete someone else Account Type."));
		}
	}
}
