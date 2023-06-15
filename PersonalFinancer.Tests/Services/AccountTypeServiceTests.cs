namespace PersonalFinancer.Tests.Services
{
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Data.Repositories;
    using PersonalFinancer.Services.ApiService;
    using PersonalFinancer.Services.ApiService.Models;

    internal class AccountTypeServiceTests : ServicesUnitTestsBase
    {
        private IEfRepository<AccountType> repo;
        private ApiService<AccountType> accountTypeService;

        [SetUp]
        public void SetUp()
        {
            this.repo = new EfRepository<AccountType>(this.sqlDbContext);
            this.accountTypeService = new ApiService<AccountType>(this.repo, this.mapper, this.memoryCache);
        }

        [Test]
        public async Task CreateEntity_ShouldAddNewAccountType_WithValidParams()
        {
            //Arrange

            string accountTypeName = "NewAccountType";
            Guid ownerId = this.User1.Id;
            int countBefore = await this.repo.All().CountAsync();

            //Act
            ApiOutputServiceModel actual =
                await this.accountTypeService.CreateEntity(accountTypeName, ownerId);

            int countAfter = await this.repo.All().CountAsync();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(countAfter, Is.EqualTo(countBefore + 1));
                Assert.That(actual, Is.Not.Null);
                Assert.That(actual.Id, Is.Not.EqualTo(Guid.Empty));
                Assert.That(actual.Name, Is.EqualTo(accountTypeName));
            });
        }

        [Test]
        public async Task CreateEntity_ShouldRecreateDeletedBeforeAccountType_WithValidParams()
        {
            //Arrange
            var deletedAccType = new AccountType
            {
                Id = Guid.NewGuid(),
                Name = "DeletedAccType",
                OwnerId = this.User1.Id,
                IsDeleted = true
            };

            await this.repo.AddAsync(deletedAccType);
            await this.repo.SaveChangesAsync();
            int countBefore = await this.repo.All().CountAsync();
            string accountTypeName = deletedAccType.Name;
            Guid ownerId = this.User1.Id;

            //Assert
            AccountType? deletedAcc = await this.repo.FindAsync(deletedAccType.Id);
            Assert.That(deletedAcc, Is.Not.Null);
            Assert.That(deletedAcc.IsDeleted, Is.True);

            //Act
            ApiOutputServiceModel result =
                await this.accountTypeService.CreateEntity(accountTypeName, ownerId);
            int countAfter = await this.repo.All().CountAsync();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(countAfter, Is.EqualTo(countBefore));
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Id, Is.EqualTo(deletedAccType.Id));
                Assert.That(result.Name, Is.EqualTo(deletedAccType.Name));
            });
        }

        [Test]
        public async Task CreateEntity_ShouldAddNewAccTypeWhenAnotherUserHaveTheSameAccType()
        {
            //Arrange
            var user2AccType = new AccountType
            {
                Id = Guid.NewGuid(),
                Name = "User2AccType",
                OwnerId = this.User2.Id
            };

            await this.repo.AddAsync(user2AccType);
            await this.repo.SaveChangesAsync();
            int countBefore = await this.repo.All().CountAsync();

            string accountTypeName = user2AccType.Name;
            Guid ownerId = this.User1.Id;

            //Assert
            Assert.That(await this.repo.FindAsync(user2AccType.Id), Is.Not.Null);

            //Act
            ApiOutputServiceModel result =
                await this.accountTypeService.CreateEntity(accountTypeName, ownerId);

            int countAfter = await this.repo.All().CountAsync();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(countAfter, Is.EqualTo(countBefore + 1));
                Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
                Assert.That(result.Id, Is.Not.EqualTo(user2AccType.Id));
                Assert.That(result.Name, Is.EqualTo(user2AccType.Name));
            });
        }

        [Test]
        public void CreateEntity_ShouldThrowException_WhenAccTypeExist()
        {
            //Arrange
            string accountTypeName = this.AccType1User1.Name;
            Guid ownerId = this.User1.Id;

            //Act & Assert
            Assert.That(async () => await this.accountTypeService.CreateEntity(accountTypeName, ownerId),
            Throws.TypeOf<ArgumentException>().With.Message.EqualTo("Entity with the same name exist."));
        }

        [Test]
        public async Task DeleteEntity_ShouldMarkAccTypeAsDeleted_WithValidParams()
        {
            //Arrange
            var newAccType = new AccountType()
            {
                Id = Guid.NewGuid(),
                Name = "NewAccType",
                OwnerId = this.User1.Id
            };
            await this.repo.AddAsync(newAccType);
            await this.repo.SaveChangesAsync();

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(await this.repo.FindAsync(newAccType.Id), Is.Not.Null);
                Assert.That(newAccType.IsDeleted, Is.False);
            });

            //Act
            await this.accountTypeService.DeleteEntity(newAccType.Id, this.User1.Id, isUserAdmin: false);

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(newAccType.IsDeleted, Is.True);
                Assert.That(await this.repo.FindAsync(newAccType.Id), Is.Not.Null);
            });
        }

        [Test]
        public async Task DeleteEntity_ShouldMarkAccTypeAsDeleted_WhenUserIsAdmin()
        {
            //Arrange
            var newAccType = new AccountType()
            {
                Id = Guid.NewGuid(),
                Name = "NewAccType",
                OwnerId = this.User1.Id
            };
            await this.repo.AddAsync(newAccType);
            await this.repo.SaveChangesAsync();

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(await this.repo.FindAsync(newAccType.Id), Is.Not.Null);
                Assert.That(newAccType.IsDeleted, Is.False);
            });

            //Act
            await this.accountTypeService.DeleteEntity(newAccType.Id, this.User2.Id, isUserAdmin: true);

            //Assert
            Assert.Multiple(async () =>
            {
                Assert.That(newAccType.IsDeleted, Is.True);
                Assert.That(await this.repo.FindAsync(newAccType.Id), Is.Not.Null);
            });
        }

        [Test]
        public void DeleteEntity_ShouldThrowException_WhenAccTypeNotExist()
        {
            //Act & Assert
            Assert.That(async () => await this.accountTypeService
                  .DeleteEntity(Guid.NewGuid(), this.User1.Id, isUserAdmin: false),
            Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public async Task DeleteEntity_ShouldThrowException_WhenUserIsNotOwner()
        {
            //Arrange
            var user2AccType = new AccountType()
            {
                Id = Guid.NewGuid(),
                Name = "ForDelete",
                OwnerId = this.User2.Id
            };
            await this.repo.AddAsync(user2AccType);
            await this.repo.SaveChangesAsync();

            //Act & Assert
            Assert.That(async () => await this.accountTypeService
                  .DeleteEntity(user2AccType.Id, this.User1.Id, isUserAdmin: false),
            Throws.TypeOf<ArgumentException>().With.Message.EqualTo("Unauthorized."));
        }
    }
}
