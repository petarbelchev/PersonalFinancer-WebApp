using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.ApiService;
using PersonalFinancer.Services.ApiService.Models;

namespace PersonalFinancer.Tests.Services
{
	internal class AccountTypeServiceTests : UnitTestsBase
	{
		private ApiService<AccountType> accountTypeService;

		[SetUp]
		public void SetUp()
		{
			this.accountTypeService = new ApiService<AccountType>(this.sqlDbContext, this.mapper, this.memoryCache);
		}

		[Test]
		public async Task CreateEntity_ShouldAddNewAccountType_WithValidParams()
		{
			//Arrange
			var inputModel = new ApiInputServiceModel
			{
				Name = "NewAccountType",
				OwnerId = this.User1.Id
			};
			int countBefore = await sqlDbContext.AccountTypes.CountAsync();

			//Act
			ApiOutputServiceModel actual =
				await accountTypeService.CreateEntity(inputModel);

			int countAfter = await sqlDbContext.AccountTypes.CountAsync();

			//Assert
			Assert.That(countAfter, Is.EqualTo(countBefore + 1));
			Assert.That(actual, Is.Not.Null);
			Assert.That(actual.Id, Is.Not.Null);
			Assert.That(actual.Name, Is.EqualTo(inputModel.Name));
		}

		[Test]
		public async Task CreateEntity_ShouldRecreateDeletedBeforeAccountType_WithValidParams()
		{
			//Arrange
			var deletedAccType = new AccountType
			{
				Id = Guid.NewGuid().ToString(),
				Name = "DeletedAccType",
				OwnerId = this.User1.Id,
				IsDeleted = true
			};
			await sqlDbContext.AccountTypes.AddAsync(deletedAccType);
			await sqlDbContext.SaveChangesAsync();
			int countBefore = await sqlDbContext.AccountTypes.CountAsync();

			var inputModel = new ApiInputServiceModel
			{
				Name = deletedAccType.Name,
				OwnerId = this.User1.Id
			};

			//Assert
			Assert.That(async () =>
			{
				var deletedAcc = await sqlDbContext.AccountTypes.FindAsync(deletedAccType.Id);
				Assert.That(deletedAcc, Is.Not.Null);
				return deletedAcc.IsDeleted;
			},
			Is.True);

			//Act
			ApiOutputServiceModel result =
				await accountTypeService.CreateEntity(inputModel);

			int countAfter = await sqlDbContext.AccountTypes.CountAsync();

			//Assert
			Assert.That(countAfter, Is.EqualTo(countBefore));
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Id, Is.EqualTo(deletedAccType.Id));
			Assert.That(result.Name, Is.EqualTo(deletedAccType.Name));
		}

		[Test]
		public async Task CreateEntity_ShouldAddNewAccTypeWhenAnotherUserHaveTheSameAccType()
		{
			//Arrange
			var user2AccType = new AccountType
			{
				Id = Guid.NewGuid().ToString(),
				Name = "User2AccType",
				OwnerId = this.User2.Id
			};

			await sqlDbContext.AccountTypes.AddAsync(user2AccType);
			await sqlDbContext.SaveChangesAsync();
			int countBefore = await sqlDbContext.AccountTypes.CountAsync();

			var inputModel = new ApiInputServiceModel
			{
				Name = user2AccType.Name,
				OwnerId = this.User1.Id
			};

			//Assert
			Assert.That(await sqlDbContext.AccountTypes.FindAsync(user2AccType.Id), Is.Not.Null);

			//Act
			ApiOutputServiceModel result =
				await accountTypeService.CreateEntity(inputModel);

			int countAfter = await sqlDbContext.AccountTypes.CountAsync();

			//Assert
			Assert.That(countAfter, Is.EqualTo(countBefore + 1));
			Assert.That(result.Id, Is.Not.Null);
			Assert.That(result.Id, Is.Not.EqualTo(user2AccType.Id));
			Assert.That(result.Name, Is.EqualTo(user2AccType.Name));
		}

		[Test]
		public void CreateEntity_ShouldThrowException_WhenAccTypeExist()
		{
			//Arrange
			var inputModel = new ApiInputServiceModel
			{
				Name = this.AccType1User1.Name,
				OwnerId = this.User1.Id
			};

			//Act & Assert
			Assert.That(async () => await accountTypeService.CreateEntity(inputModel),
				Throws.TypeOf<ArgumentException>().With.Message
					.EqualTo("Entity with the same name exist."));
		}

		[Test]
		public async Task DeleteEntity_ShouldMarkAccTypeAsDeleted_WithValidParams()
		{
			//Arrange
			var newAccType = new AccountType()
			{
				Id = Guid.NewGuid().ToString(),
				Name = "NewAccType",
				OwnerId = this.User1.Id
			};
			await sqlDbContext.AccountTypes.AddAsync(newAccType);
			await sqlDbContext.SaveChangesAsync();

			//Assert
			Assert.That(await sqlDbContext.AccountTypes.FindAsync(newAccType.Id), Is.Not.Null);
			Assert.That(newAccType.IsDeleted, Is.False);

			//Act
			await accountTypeService.DeleteEntity(newAccType.Id, this.User1.Id, isUserAdmin: false);

			//Assert
			Assert.That(newAccType.IsDeleted, Is.True);
			Assert.That(await sqlDbContext.AccountTypes.FindAsync(newAccType.Id), Is.Not.Null);
		}
		
		[Test]
		public async Task DeleteEntity_ShouldMarkAccTypeAsDeleted_WhenUserIsAdmin()
		{
			//Arrange
			var newAccType = new AccountType()
			{
				Id = Guid.NewGuid().ToString(),
				Name = "NewAccType",
				OwnerId = this.User1.Id
			};
			await sqlDbContext.AccountTypes.AddAsync(newAccType);
			await sqlDbContext.SaveChangesAsync();

			//Assert
			Assert.That(await sqlDbContext.AccountTypes.FindAsync(newAccType.Id), Is.Not.Null);
			Assert.That(newAccType.IsDeleted, Is.False);

			//Act
			await accountTypeService.DeleteEntity(newAccType.Id, this.User2.Id, isUserAdmin: true);

			//Assert
			Assert.That(newAccType.IsDeleted, Is.True);
			Assert.That(await sqlDbContext.AccountTypes.FindAsync(newAccType.Id), Is.Not.Null);
		}

		[Test]
		public void DeleteEntity_ShouldThrowException_WhenAccTypeNotExist()
		{
			//Act & Assert
			Assert.That(async () => await accountTypeService.DeleteEntity(
				Guid.NewGuid().ToString(), this.User1.Id, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task DeleteEntity_ShouldThrowException_WhenUserIsNotOwner()
		{
			//Arrange
			var user2AccType = new AccountType()
			{
				Id = Guid.NewGuid().ToString(),
				Name = "ForDelete",
				OwnerId = this.User2.Id
			};
			await sqlDbContext.AccountTypes.AddAsync(user2AccType);
			await sqlDbContext.SaveChangesAsync();

			//Act & Assert
			Assert.That(async () => await accountTypeService
				.DeleteEntity(user2AccType.Id, this.User1.Id, isUserAdmin: false),
			Throws.TypeOf<ArgumentException>()
					.With.Message.EqualTo("Unauthorized."));
		}
	}
}
