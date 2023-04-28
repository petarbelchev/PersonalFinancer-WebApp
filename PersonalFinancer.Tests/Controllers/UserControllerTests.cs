//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.ViewFeatures;

//using Moq;
//using NUnit.Framework;

//using PersonalFinancer.Data.Models;
//using PersonalFinancer.Tests.Mocks;
//using PersonalFinancer.Web.Controllers;
//using PersonalFinancer.Web.Models.User;
//using static PersonalFinancer.Data.Constants.RoleConstants;

//namespace PersonalFinancer.Tests.Controllers
//{
//	[TestFixture]
//	internal class UserControllerTests
//	{
//		private UserController controller;
//		private Mock<UserManager<ApplicationUser>> userManagerMock;
//		private Mock<SignInManager<ApplicationUser>> signInManagerMock;

//		[SetUp]
//		public void SetUp()
//		{
//			userManagerMock = new Mock<UserManager<ApplicationUser>>(
//				/* IUserStore<TUser> store */Mock.Of<IUserStore<ApplicationUser>>(),
//				/* IOptions<IdentityOptions> optionsAccessor */null,
//				/* IPasswordHasher<TUser> passwordHasher */null,
//				/* IEnumerable<IUserValidator<TUser>> userValidators */null,
//				/* IEnumerable<IPasswordValidator<TUser>> passwordValidators */null,
//				/* ILookupNormalizer keyNormalizer */null,
//				/* IdentityErrorDescriber errors */null,
//				/* IServiceProvider services */null,
//				/* ILogger<UserManager<TUser>> logger */null);

//			signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
//				userManagerMock.Object,
//				/* IHttpContextAccessor contextAccessor */Mock.Of<IHttpContextAccessor>(),
//				/* IUserClaimsPrincipalFactory<TUser> claimsFactory */Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
//				/* IOptions<IdentityOptions> optionsAccessor */null,
//				/* ILogger<SignInManager<TUser>> logger */null,
//				/* IAuthenticationSchemeProvider schemes */null,
//				/* IUserConfirmation<TUser> confirmation */null);

//			controller = new UserController(
//				userManagerMock.Object, signInManagerMock.Object, ControllersMapperMock.Instance);
//		}

//		[Test]
//		public void Register_ShouldReturnCorrectViewModel()
//		{
//			//Arrange
//			var expect = new RegisterFormViewModel();

//			//Act
//			ViewResult viewResult = (ViewResult)controller.Register();
//			var actual = viewResult.Model as RegisterFormViewModel;

//			//Assert
//			Assert.That(actual, Is.Not.Null);
//			Assert.That(actual.FirstName, Is.EqualTo(expect.FirstName));
//			Assert.That(actual.LastName, Is.EqualTo(expect.LastName));
//			Assert.That(actual.Password, Is.EqualTo(expect.Password));
//			Assert.That(actual.Email, Is.EqualTo(expect.Email));
//			Assert.That(actual.PhoneNumber, Is.EqualTo(expect.PhoneNumber));
//			Assert.That(actual.ConfirmPassword, Is.EqualTo(expect.ConfirmPassword));
//		}

//		[Test]
//		public async Task Register_ShouldRegisterUser_WithValidInputModel()
//		{
//			//Arrange
//			var inputModel = new RegisterFormViewModel
//			{
//				FirstName = "Test First Name",
//				LastName = "Test Last Name",
//				Email = "test@mail.com",
//				Password = "password",
//				ConfirmPassword = "password",
//				PhoneNumber = "0123456789"
//			};

//			userManagerMock
//				.Setup(x => x.CreateAsync(
//					It.Is<ApplicationUser>(u => u.FirstName == inputModel.FirstName
//												&& u.LastName == inputModel.LastName
//												&& u.Email == inputModel.Email
//												&& u.PhoneNumber == inputModel.PhoneNumber),
//					It.Is<string>(password => password == inputModel.Password)))
//				.ReturnsAsync(IdentityResult.Success);
			
//			userManagerMock
//				.Setup(x => x.AddToRoleAsync(
//					It.Is<ApplicationUser>(u => u.FirstName == inputModel.FirstName
//												&& u.LastName == inputModel.LastName
//												&& u.Email == inputModel.Email
//												&& u.PhoneNumber == inputModel.PhoneNumber),
//					It.Is<string>(role => role == UserRoleName)))
//				.ReturnsAsync(IdentityResult.Success);

//			controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

//			//Act
//			var result = (RedirectToActionResult)await controller.Register(inputModel);

//			signInManagerMock.Verify(x => x.SignInAsync(
//					It.Is<ApplicationUser>(u => 
//						u.FirstName == inputModel.FirstName
//						&& u.LastName == inputModel.LastName
//						&& u.Email == inputModel.Email
//						&& u.PhoneNumber == inputModel.PhoneNumber), 
//					false, null), 
//				Times.Once);

//			//Assert
//			Assert.That(result, Is.Not.Null);
//			Assert.That(result.ControllerName, Is.EqualTo("Home"));
//			Assert.That(result.ActionName, Is.EqualTo("Index"));
//			Assert.That(controller.TempData.Keys.Count, Is.EqualTo(1));
//			Assert.That(controller.TempData.Keys.First(), Is.EqualTo("successMsg"));
//			Assert.That(controller.TempData.Values.Count, Is.EqualTo(1));
//			Assert.That(controller.TempData.Values.First(), Is.EqualTo("Congratulations! Your registration was successful!"));
//		}

//		[Test]
//		public async Task Register_ShouldReturnViewModelWithModelErrors_WhenCreationWasUnsuccsessful()
//		{
//			//Arrange
//			var inputModel = new RegisterFormViewModel
//			{
//				FirstName = "Test First Name",
//				LastName = "Test Last Name",
//				Email = "test@mail.com",
//				Password = "password",
//				ConfirmPassword = "password",
//				PhoneNumber = "0123456789"
//			};			

//			var error = new IdentityError();
//			error.Code = "Test Code";
//			error.Description = "Test Description";

//			userManagerMock.Setup(x => 
//				x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
//				.ReturnsAsync(IdentityResult.Failed(error));
			
//			//Act
//			ViewResult viewResult = (ViewResult)await controller.Register(inputModel);

//			//Assert
//			Assert.That(viewResult, Is.Not.Null);
//			Assert.That(viewResult.ViewData.ModelState.Values.Count(), Is.EqualTo(1));

//			Assert.That(viewResult.ViewData.ModelState.Keys.First(), Is.EqualTo(error.Code));

//			Assert.That(viewResult.ViewData.ModelState.Values.First().Errors.First().ErrorMessage, 
//				Is.EqualTo(error.Description));

//			var model = viewResult.Model as RegisterFormViewModel;
//			Assert.That(model, Is.Not.Null);
//			Assert.That(model.Email, Is.EqualTo(inputModel.Email));
//			Assert.That(model.Password, Is.EqualTo(inputModel.Password));
//			Assert.That(model.FirstName, Is.EqualTo(inputModel.FirstName));
//			Assert.That(model.LastName, Is.EqualTo(inputModel.LastName));
//			Assert.That(model.ConfirmPassword, Is.EqualTo(inputModel.ConfirmPassword));
//			Assert.That(model.PhoneNumber, Is.EqualTo(inputModel.PhoneNumber));
//		}

//		[Test]
//		public async Task Register_ShouldReturnViewModelWithModelErrors_WhenModelWasInvalid()
//		{
//			//Arrange
//			var inputModel = new RegisterFormViewModel
//			{
//				FirstName = "Test First Name",
//				LastName = "Test Last Name",
//				Email = "invalid email",
//				Password = "password",
//				ConfirmPassword = "password",
//				PhoneNumber = "0123456789"
//			};

//			controller.ModelState.AddModelError(nameof(inputModel.Email), "Email is invalid.");

//			//Act
//			ViewResult viewResult = (ViewResult)await controller.Register(inputModel);

//			//Assert
//			Assert.That(viewResult, Is.Not.Null);
//			Assert.That(viewResult.ViewData.ModelState.Values.Count(), Is.EqualTo(1));

//			Assert.That(viewResult.ViewData.ModelState.Keys.First(), Is.EqualTo(nameof(inputModel.Email)));

//			Assert.That(viewResult.ViewData.ModelState.Values.First().Errors.First().ErrorMessage, 
//				Is.EqualTo("Email is invalid."));

//			var model = viewResult.Model as RegisterFormViewModel;
//			Assert.That(model, Is.Not.Null);
//			Assert.That(model.Email, Is.EqualTo(inputModel.Email));
//			Assert.That(model.Password, Is.EqualTo(inputModel.Password));
//			Assert.That(model.FirstName, Is.EqualTo(inputModel.FirstName));
//			Assert.That(model.LastName, Is.EqualTo(inputModel.LastName));
//			Assert.That(model.ConfirmPassword, Is.EqualTo(inputModel.ConfirmPassword));
//			Assert.That(model.PhoneNumber, Is.EqualTo(inputModel.PhoneNumber));
//		}

//		[Test]
//		public async Task Register_ShouldReturnViewModelWithModelErrors_WhenAddToRoleWasUnsuccsessful()
//		{
//			//Arrange
//			var inputModel = new RegisterFormViewModel
//			{
//				FirstName = "Test First Name",
//				LastName = "Test Last Name",
//				Email = "test@mail.com",
//				Password = "password",
//				ConfirmPassword = "password",
//				PhoneNumber = "0123456789"
//			};

//			userManagerMock
//				.Setup(x => x.CreateAsync(
//					It.Is<ApplicationUser>(u => u.FirstName == inputModel.FirstName
//												&& u.LastName == inputModel.LastName
//												&& u.Email == inputModel.Email
//												&& u.PhoneNumber == inputModel.PhoneNumber),
//					It.Is<string>(password => password == inputModel.Password)))
//				.ReturnsAsync(IdentityResult.Success);
			
//			var error = new IdentityError();
//			error.Code = "Test Code";
//			error.Description = "Test Description";

//			userManagerMock
//				.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
//				.ReturnsAsync(IdentityResult.Failed(error));

//			//Act
//			ViewResult viewResult = (ViewResult)await controller.Register(inputModel);

//			userManagerMock.Verify(x => x.DeleteAsync(
//				It.Is<ApplicationUser>(u => u.FirstName == inputModel.FirstName
//											&& u.LastName == inputModel.LastName
//											&& u.Email == inputModel.Email
//											&& u.PhoneNumber == inputModel.PhoneNumber)), 
//				Times.Once);

//			//Assert
//			Assert.That(viewResult, Is.Not.Null);
//			Assert.That(viewResult.ViewData.ModelState.Values.Count(), Is.EqualTo(1));

//			Assert.That(viewResult.ViewData.ModelState.Keys.First(), Is.EqualTo(error.Code));

//			Assert.That(viewResult.ViewData.ModelState.Values.First().Errors.First().ErrorMessage, 
//				Is.EqualTo(error.Description));

//			var model = viewResult.Model as RegisterFormViewModel;
//			Assert.That(model, Is.Not.Null);
//			Assert.That(model.Email, Is.EqualTo(inputModel.Email));
//			Assert.That(model.Password, Is.EqualTo(inputModel.Password));
//			Assert.That(model.FirstName, Is.EqualTo(inputModel.FirstName));
//			Assert.That(model.LastName, Is.EqualTo(inputModel.LastName));
//			Assert.That(model.ConfirmPassword, Is.EqualTo(inputModel.ConfirmPassword));
//			Assert.That(model.PhoneNumber, Is.EqualTo(inputModel.PhoneNumber));
//		}
				
//		[Test]
//		public void Login_ShouldReturnCorrectViewModel()
//		{
//			//Arrange
//			var expect = new LoginFormViewModel();

//			//Act
//			ViewResult viewResult = (ViewResult)controller.Login();
//			var actual = viewResult.Model as LoginFormViewModel;

//			//Assert
//			Assert.That(actual, Is.Not.Null);
//			Assert.That(actual.Email, Is.EqualTo(expect.Email));
//			Assert.That(actual.Password, Is.EqualTo(expect.Password));
//			Assert.That(actual.RememberMe, Is.EqualTo(expect.RememberMe));
//		}

//		[Test]
//		public async Task Login_ShouldLoginUserWithoutRemember_WhenInputModelIsValid()
//		{
//			//Arrange
//			var inputModel = new LoginFormViewModel
//			{
//				Email = "test@mail.com",
//				Password = "testPass",
//				RememberMe = false
//			};

//			signInManagerMock.Setup(x => x.PasswordSignInAsync(
//					It.Is<string>(x => x == inputModel.Email), 
//					It.Is<string>(x => x == inputModel.Password), 
//					It.Is<bool>(x => x == false),
//					It.Is<bool>(x => x == false)))
//				.ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

//			//Act
//			var result = (RedirectToActionResult)await controller.Login(inputModel);

//			//Assert
//			Assert.That(result, Is.Not.Null);
//			Assert.That(result.ControllerName, Is.EqualTo("Home"));
//			Assert.That(result.ActionName, Is.EqualTo("Index"));
//		}

//		[Test]
//		public async Task Login_ShouldReturnViewModelWithModelErrors_WhenModelWasInvalid()
//		{
//			//Arrange
//			var inputModel = new LoginFormViewModel
//			{
//				Email = "invalid email",
//				Password = "testPass",
//				RememberMe = false
//			};
			
//			controller.ModelState.AddModelError(nameof(inputModel.Email), "Email is invalid.");

//			//Act
//			ViewResult viewResult = (ViewResult)await controller.Login(inputModel);
			
//			//Assert
//			Assert.That(viewResult, Is.Not.Null);
//			Assert.That(viewResult.ViewData.ModelState.Values.Count(), Is.EqualTo(1));

//			Assert.That(viewResult.ViewData.ModelState.Keys.First(), Is.EqualTo(nameof(inputModel.Email)));

//			Assert.That(viewResult.ViewData.ModelState.Values.First().Errors.First().ErrorMessage, 
//				Is.EqualTo("Email is invalid."));

//			var model = viewResult.Model as LoginFormViewModel;
//			Assert.That(model, Is.Not.Null);
//			Assert.That(model.Email, Is.EqualTo(inputModel.Email));
//			Assert.That(model.Password, Is.EqualTo(inputModel.Password));
//		}

//		[Test]
//		public async Task Login_ShouldReturnViewModelWithModelErrors_WhenLoginWasUnsuccsessful()
//		{
//			//Arrange
//			var inputModel = new LoginFormViewModel
//			{
//				Email = "test@mail.com",
//				Password = "testPass",
//				RememberMe = false
//			};
			
//			signInManagerMock.Setup(x => x.PasswordSignInAsync(
//					It.IsAny<string>(),	It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
//				.ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

//			//Act
//			var viewResult = (ViewResult)await controller.Login(inputModel);

//			//Assert
//			Assert.That(viewResult, Is.Not.Null);
//			Assert.That(viewResult.ViewData.ModelState.Values.Count(), Is.EqualTo(1));

//			Assert.That(viewResult.ViewData.ModelState.Keys.First(), Is.EqualTo(string.Empty));

//			Assert.That(viewResult.ViewData.ModelState.Values.First().Errors.First().ErrorMessage, 
//				Is.EqualTo("Invalid login attempt."));

//			var model = viewResult.Model as LoginFormViewModel;
//			Assert.That(model, Is.Not.Null);
//			Assert.That(model.Email, Is.EqualTo(inputModel.Email));
//			Assert.That(model.Password, Is.EqualTo(inputModel.Password));
//		}

//		[Test]
//		public async Task Logout_ShouldLogoutUser()
//		{
//			//Act
//			var result = (RedirectToActionResult)await controller.Logout();

//			signInManagerMock.Verify(x => x.SignOutAsync(), Times.Once);

//			//Assert
//			Assert.That(result, Is.Not.Null);
//			Assert.That(result.ControllerName, Is.EqualTo("Home"));
//			Assert.That(result.ActionName, Is.EqualTo("Index"));
//		}
//	}
//}
