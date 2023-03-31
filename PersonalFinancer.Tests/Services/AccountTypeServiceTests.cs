using NUnit.Framework;

using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.AccountTypes;
using PersonalFinancer.Services.AccountTypes.Models;
using PersonalFinancer.Services.Shared.Models;

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
		public async Task CreateAccountType_ShouldAddNewAccountType_WithValidParams()
		{
			//Arrange
			var inputModel = new AccountTypeInputModel
			{
				Name = "NewAccountType",
				OwnerId = this.User1.Id
			};
			int countBefore = data.AccountTypes.Count();

			//Act
			AccountTypeViewModel viewModel =
				await accountTypeService.CreateAccountType(inputModel);

			int countAfter = data.AccountTypes.Count();

			//Assert
			Assert.That(countAfter, Is.EqualTo(countBefore + 1));
			Assert.That(viewModel.Id, Is.Not.Null);
			Assert.That(viewModel.Name, Is.EqualTo(inputModel.Name));
		}

		[Test]
		public async Task CreateAccountType_ShouldRecreateDeletedBeforeAccountType_WithValidParams()
		{
			//Arrange
			var deletedAccType = new AccountType
			{
				Id = Guid.NewGuid().ToString(),
				Name = "DeletedAccType",
				OwnerId = this.User1.Id,
				IsDeleted = true
			};
			await data.AccountTypes.AddAsync(deletedAccType);
			await data.SaveChangesAsync();
			int countBefore = data.AccountTypes.Count();

			var inputModel = new AccountTypeInputModel
			{
				Name = deletedAccType.Name,
				OwnerId = this.User1.Id
			};

			//Assert
			Assert.That(async () =>
			{
				var deletedAcc = await data.AccountTypes.FindAsync(deletedAccType.Id);
				Assert.That(deletedAcc, Is.Not.Null);
				return deletedAcc.IsDeleted;
			}, Is.True);

			//Act
			AccountTypeViewModel viewModel =
				await accountTypeService.CreateAccountType(inputModel);

			int countAfter = data.AccountTypes.Count();

			//Assert
			Assert.That(countAfter, Is.EqualTo(countBefore));
			Assert.That(viewModel.Id, Is.Not.Null);
			Assert.That(viewModel.Name, Is.EqualTo(deletedAccType.Name));
		}

		[Test]
		public async Task CreateAccountType_ShouldAddNewAccTypeWhenAnotherUserHaveTheSameAccType()
		{
			//Arrange
			var user2AccType = new AccountType
			{
				Id = Guid.NewGuid().ToString(),
				Name = "User2AccType",
				OwnerId = this.User2.Id
			};

			await data.AccountTypes.AddAsync(user2AccType);
			await data.SaveChangesAsync();
			int countBefore = data.AccountTypes.Count();

			var inputModel = new AccountTypeInputModel
			{
				Name = user2AccType.Name,
				OwnerId = this.User1.Id
			};

			//Assert
			Assert.That(await data.AccountTypes.FindAsync(user2AccType.Id), Is.Not.Null);

			//Act
			AccountTypeViewModel viewModel =
				await accountTypeService.CreateAccountType(inputModel);

			int countAfter = data.AccountTypes.Count();

			//Assert
			Assert.That(countAfter, Is.EqualTo(countBefore + 1));
			Assert.That(viewModel.Id, Is.Not.Null);
			Assert.That(viewModel.Name, Is.EqualTo("User2AccType"));
		}

		[Test]
		public void CreateAccountType_ShouldThrowException_WhenAccTypeExist()
		{
			//Arrange

			var inputModel = new AccountTypeInputModel
			{
				Name = this.AccountType1.Name,
				OwnerId = this.User1.Id
			};

			//Act & Assert
			Assert.That(async () => await accountTypeService.CreateAccountType(inputModel),
				Throws.TypeOf<ArgumentException>().With.Message.EqualTo("Account Type with the same name exist."));
		}

		//[Test]
		//[TestCase("A")]
		//[TestCase("NameWith16Chars!")]
		//public void CreateAccountType_ShouldThrowException_WithInvalidName(string accountTypeName)
		//{
		//	//Arrange
		//	var inputModel = new AccountTypeInputModel
		//	{
		//		Name = accountTypeName,
		//		OwnerId = this.User1.Id
		//	};

		//	//Act & Assert
		//	Assert.That(async () => await accountService.CreateAccountType(inputModel),
		//		Throws.TypeOf<ArgumentException>().With.Message
		//			.EqualTo("Account Type name must be between 2 and 15 characters long."));
		//}

		[Test]
		public async Task DeleteAccountType_ShouldRemoveAccType_WithValidParams()
		{
			//Arrange
			var newAccType = new AccountType()
			{
				Id = Guid.NewGuid().ToString(),
				Name = "NewAccType",
				OwnerId = this.User1.Id
			};
			await data.AccountTypes.AddAsync(newAccType);
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
			var user2AccType = new AccountType()
			{
				Id = Guid.NewGuid().ToString(),
				Name = "ForDelete",
				OwnerId = this.User2.Id
			};
			await data.AccountTypes.AddAsync(user2AccType);
			await data.SaveChangesAsync();

			//Act & Assert
			Assert.That(async () => await accountTypeService
					.DeleteAccountType(user2AccType.Id, this.User1.Id),
				Throws.TypeOf<ArgumentException>()
					.With.Message.EqualTo("Can't delete someone else Account Type."));
		}
	}
}
