namespace PersonalFinancer.Tests.Services
{
	using Microsoft.EntityFrameworkCore;
	using NUnit.Framework;
	using PersonalFinancer.Common.Messages;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Repositories;
	using PersonalFinancer.Services.Api;
	using PersonalFinancer.Services.Api.Models;

	internal class AccountTypeServiceTests : ServicesUnitTestsBase
	{
		private IEfRepository<AccountType> repo;
		private ApiService<AccountType> accountTypeService;

		[SetUp]
		public void SetUp()
		{
			this.repo = new EfRepository<AccountType>(this.dbContext);
			this.accountTypeService = new ApiService<AccountType>(
				this.repo, this.mapper, this.memoryCache);
		}

		[Test]
		public async Task CreateEntityAsync_ShouldAddNewAccountType()
		{
			//Arrange
			string accountTypeName = "NewAccountType";
			int countBefore = await this.repo.All().CountAsync();

			//Act
			ApiEntityDTO actual = await this.accountTypeService
				.CreateEntityAsync(accountTypeName, this.mainTestUserId);

			//Arrange
			int countAfter = await this.repo.All().CountAsync();

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(countAfter, Is.EqualTo(countBefore + 1));
				Assert.That(actual.Id, Is.Not.EqualTo(Guid.Empty));
				Assert.That(actual.Name, Is.EqualTo(accountTypeName));
			});
		}

		[Test]
		public async Task CreateEntityAsync_ShouldRecreateDeletedAccountType()
		{
			//Arrange
			AccountType deletedAccType = await this.repo.All()
				.Where(c => c.IsDeleted && c.OwnerId == this.mainTestUserId)
				.FirstAsync();

			int countBefore = await this.repo.All().CountAsync();

			//Act
			ApiEntityDTO result = await this.accountTypeService
				.CreateEntityAsync(deletedAccType.Name, this.mainTestUserId);

			//Arrange
			int countAfter = await this.repo.All().CountAsync();

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(countAfter, Is.EqualTo(countBefore));

				AssertSamePropertiesValuesAreEqual(result, deletedAccType);
			});
		}

		[Test]
		public async Task CreateEntityAsync_ShouldAddNewAccTypeWhenAnotherUserHaveTheSameAccType()
		{
			//Arrange
			AccountType anotherUserAccountType = await this.repo.All()
				.Where(c => c.OwnerId != this.mainTestUserId)
				.FirstAsync();

			int countBefore = await this.repo.All().CountAsync();

			//Act
			ApiEntityDTO result = await this.accountTypeService
				.CreateEntityAsync(anotherUserAccountType.Name, this.mainTestUserId);

			//Arrange
			int countAfter = await this.repo.All().CountAsync();

			//Assert
			Assert.Multiple(() =>
			{
				Assert.That(countAfter, Is.EqualTo(countBefore + 1));
				Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
				Assert.That(result.Id, Is.Not.EqualTo(anotherUserAccountType.Id));
				Assert.That(result.Name, Is.EqualTo(anotherUserAccountType.Name));
			});
		}

		[Test]
		public async Task CreateEntityAsync_ShouldThrowArgumentException_WhenTheAccountTypeExistAndIsNotDeleted()
		{
			//Arrange
			string existingName = await this.repo.All()
				.Where(c => c.OwnerId == this.mainTestUserId && !c.IsDeleted)
				.Select(c => c.Name)
				.FirstAsync();

			//Act & Assert
			Assert.That(async () => await this.accountTypeService.CreateEntityAsync(existingName, this.mainTestUserId),
			Throws.TypeOf<ArgumentException>().With.Message.EqualTo(ExceptionMessages.ExistingEntityName));
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public async Task DeleteEntityAsync_ShouldMarkAccountTypeAsDeleted(bool isUserAdmin)
		{
			//Arrange
			AccountType accountType = await this.repo.All()
				.Where(c => c.OwnerId == this.mainTestUserId && !c.IsDeleted)
				.FirstAsync();

			Guid currentUserId = isUserAdmin ? this.adminId : this.mainTestUserId;

			//Act
			await this.accountTypeService.DeleteEntityAsync(accountType.Id, currentUserId, isUserAdmin);

			//Assert
			Assert.Multiple(async () =>
			{
				Assert.That(accountType.IsDeleted, Is.True);
				Assert.That(await this.repo.FindAsync(accountType.Id), Is.Not.Null);
			});
		}

		[Test]
		public void DeleteEntityAsync_ShouldThrowInvalidOperationException_WhenTheAccountTypeDoesNotExist()
		{
			//Arrange
			var invalidId = Guid.NewGuid();

			//Act & Assert
			Assert.That(async () => await this.accountTypeService
				  .DeleteEntityAsync(invalidId, this.mainTestUserId, isUserAdmin: false),
			Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task DeleteEntityAsync_ShouldThrowUnauthorizedAccessException_WhenTheUserIsUnauthorized()
		{
			//Arrange
			AccountType accountType = await this.repo.All()
				.Where(c => c.OwnerId != this.mainTestUserId && !c.IsDeleted)
				.FirstAsync();

			//Act & Assert
			Assert.That(async () => await this.accountTypeService
				  .DeleteEntityAsync(accountType.Id, this.mainTestUserId, isUserAdmin: false),
			Throws.TypeOf<UnauthorizedAccessException>().With.Message.EqualTo(ExceptionMessages.UnauthorizedUser));
		}
	}
}
