using Microsoft.EntityFrameworkCore.Storage;
using NUnit.Framework;
using PersonalFinancer.Data.Models;
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
			this.accountTypeService = new AccountTypeService(this.data, this.mapper, this.memoryCache);
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

		[Test]
		public async Task CreateAccountType_ShouldAddNewAccountType_WithValidParams()
		{
			//Arrange
			int countBefore = data.AccountTypes.Count();

			//Act
			var newAccType = await accountTypeService.CreateAccountType(this.User1.Id, "NewAccountType");
			int countAfter = data.AccountTypes.Count();

			//Assert
			Assert.That(newAccType, Is.Not.Null);
			Assert.That(countAfter, Is.EqualTo(countBefore + 1));
			Assert.That(newAccType.UserId, Is.EqualTo(this.User1.Id));
			Assert.That(newAccType.Name, Is.EqualTo("NewAccountType"));
		}

		[Test]
		public async Task CreateAccountType_ShouldRecreateDeletedBeforeAccountType_WithValidParams()
		{
			//Arrange
			var deletedAccType = new AccountType
			{
				Name = "DeletedAccType",
				UserId = this.User1.Id,
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
			var newAccType = await accountTypeService.CreateAccountType(this.User1.Id, "DeletedAccType");
			int countAfter = data.AccountTypes.Count();

			//Assert
			Assert.That(newAccType, Is.Not.Null);
			Assert.That(countAfter, Is.EqualTo(countBefore));
			Assert.That(newAccType.UserId, Is.EqualTo(this.User1.Id));
			Assert.That(newAccType.Name, Is.EqualTo(deletedAccType.Name));
		}

		[Test]
		public async Task CreateAccountType_ShouldAddNewAccTypeWhenAnotherUserHaveTheSameAccType()
		{
			//Arrange
			var user2AccType = new AccountType { Name = "User2AccType", UserId = this.User2.Id };
			data.AccountTypes.Add(user2AccType);
			await data.SaveChangesAsync();
			int countBefore = data.AccountTypes.Count();

			//Assert
			Assert.That(await data.AccountTypes.FindAsync(user2AccType.Id), Is.Not.Null);

			//Act
			var newAccType = await accountTypeService.CreateAccountType(this.User1.Id, "User2AccType");
			int countAfter = data.AccountTypes.Count();

			//Assert
			Assert.That(newAccType, Is.Not.Null);
			Assert.That(countAfter, Is.EqualTo(countBefore + 1));
			Assert.That(newAccType.UserId, Is.EqualTo(this.User1.Id));
			Assert.That(newAccType.Name, Is.EqualTo("User2AccType"));
		}

		[Test]
		public void CreateAccountType_ShouldThrowException_WhenAccTypeExist()
		{
			//Act & Assert
			Assert.That(async () => await accountTypeService.CreateAccountType(this.User1.Id, this.AccountType1.Name),
				Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo("Account Type with the same name exist!"));
		}

		[Test]
		public async Task DeleteAccountType_ShouldRemoveAccType_WithValidParams()
		{
			//Arrange
			var newAccType = new AccountType() { Name = "NewAccType", UserId = this.User1.Id };
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
			Assert.That(async () => await accountTypeService
					.DeleteAccountType(Guid.NewGuid(), this.User1.Id),
				Throws.TypeOf<ArgumentNullException>()
					.With.Property("ParamName").EqualTo("Account Type does not exist."));
		}

		[Test]
		public async Task DeleteAccountType_ShouldThrowException_WhenUserIsNotOwner()
		{
			//Arrange
			var user2AccType = new AccountType() { Name = "ForDelete", UserId = this.User2.Id };
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
