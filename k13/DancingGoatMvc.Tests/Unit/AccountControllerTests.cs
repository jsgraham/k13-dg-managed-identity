using CMS.Activities.Loggers;
using CMS.Helpers;
using CMS.Tests;

using DancingGoat.Controllers;
using DancingGoat.Models.Account;
using DancingGoat.Tests.Extensions;

using NSubstitute;

using NUnit.Framework;

using TestStack.FluentMVCTesting;

namespace DancingGoat.Tests.Unit
{
    [TestFixture]
    public class AccountControllerTests : UnitTests
    {
        private const string USERNAME = "test@host.local";
        private const string PASSWORD = "password123";

        private AccountController controller;


        [SetUp]
        public void SetUp()
        {
            var activityLogMock = Substitute.For<IMembershipActivityLogger>();

            controller = new AccountController(activityLogMock, null);
        }


        [Test]
        public void Register_RendersDefaultView()
        {
            controller.WithCallTo(c => c.Register())
                       .ShouldRenderDefaultView();
        }


        [Test]
        public void Login_RendersDefaultView()
        {
            controller.WithCallTo(c => c.Login())
                       .ShouldRenderDefaultView();
        }


        [TestCase("", "DancingGoatMvc.Register.EmailUserName.Empty")]
        [TestCase("notemail", "DancingGoatMvc.General.InvalidEmail")]
        [TestCase("**************************************************************************************************************", "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public void Register_WithRegisterModel_UserNameValidationError(string value, string expectedError)
        {
            var model = CreateRegisterViewModel(userName: value);
            controller.ValidateViewModel(model);

            controller.WithCallTo(c => c.Register(model))
                       .ShouldRenderDefaultView()
                       .WithModel<RegisterViewModel>()
                       .AndModelErrorFor(m => m.UserName)
                       .ThatEquals(expectedError);
        }


        [TestCase("", "DancingGoatMvc.Register.Password.Empty")]
        [TestCase("**************************************************************************************************************", "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public void Register_WithRegisterModel_PasswordValidationError(string value, string expectedError)
        {
            var model = CreateRegisterViewModel(password: value);
            controller.ValidateViewModel(model);

            controller.WithCallTo(c => c.Register(model))
                       .ShouldRenderDefaultView()
                       .WithModel<RegisterViewModel>()
                       .AndModelErrorFor(m => m.Password)
                       .ThatEquals(expectedError);
        }


        [TestCase("", "DancingGoatMvc.Register.PasswordConfirmation.Empty")]
        [TestCase("**************************************************************************************************************", "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public void Register_WithRegisterModel_PasswordConfirmationLengthValidationError(string value, string errorMessage)
        {
            var model = CreateRegisterViewModel(passwordConfirmation: value);
            controller.ValidateViewModel(model);

            controller.WithCallTo(c => c.Register(model))
                       .ShouldRenderDefaultView()
                       .WithModel<RegisterViewModel>()
                       .AndModelErrorFor(m => m.PasswordConfirmation)
                       .ThatEquals(ResHelper.GetString(errorMessage));
        }


        [TestCase("validButDifferent", "DancingGoatMvc.Register.PasswordConfirmation.Invalid")]
        public void Register_WithRegisterModel_PasswordConfirmationValidationError(string value, string expectedError)
        {
            var model = CreateRegisterViewModel(passwordConfirmation: value);
            controller.ValidateViewModel(model);

            var x = controller.WithCallTo(c => c.Register(model)).ShouldRenderDefaultView()
                       .WithModel<RegisterViewModel>()
                       .AndModelError("").ThatEquals(expectedError);
        }


        [TestCase("", "DancingGoatMvc.Register.FirstName.Empty")]
        [TestCase("**************************************************************************************************************", "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public void Register_WithRegisterModel_FirstNameValidationError(string value, string expectedError)
        {
            var model = CreateRegisterViewModel(firstName: value);
            controller.ValidateViewModel(model);

            controller.WithCallTo(c => c.Register(model))
                       .ShouldRenderDefaultView()
                       .WithModel<RegisterViewModel>()
                       .AndModelErrorFor(m => m.FirstName)
                       .ThatEquals(expectedError);
        }


        [TestCase("", "DancingGoatMvc.Register.LastName.Empty")]
        [TestCase("**************************************************************************************************************", "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public void Register_WithRegisterModel_LastNameValidationError(string value, string expectedError)
        {
            var model = CreateRegisterViewModel(lastName: value);
            controller.ValidateViewModel(model);

            controller.WithCallTo(c => c.Register(model))
                       .ShouldRenderDefaultView()
                       .WithModel<RegisterViewModel>()
                       .AndModelErrorFor(m => m.LastName)
                       .ThatEquals(expectedError);
        }


        [TestCase("", "DancingGoatMvc.SignIn.EmailUserName.Empty")]
        [TestCase("**************************************************************************************************************", "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public void Login_WithLoginModel_UserNameValidationError(string value, string expectedError)
        {
            var model = CreateLoginViewModel(userName: value);
            controller.ValidateViewModel(model);

            controller.WithCallTo(c => c.Login(model, ""))
                       .ShouldRenderDefaultView()
                       .WithModel<LoginViewModel>()
                       .AndModelErrorFor(m => m.UserName)
                       .ThatEquals(expectedError);
        }


        [TestCase("**************************************************************************************************************", "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public void Login_WithLoginModel_PasswordValidationError(string value, string expectedError)
        {
            var model = CreateLoginViewModel(password: value);
            controller.ValidateViewModel(model);

            controller.WithCallTo(c => c.Login(model, ""))
                       .ShouldRenderDefaultView()
                       .WithModel<LoginViewModel>()
                       .AndModelErrorFor(m => m.Password)
                       .ThatEquals(expectedError);
        }


        private RegisterViewModel CreateRegisterViewModel(string userName = USERNAME, string firstName = "Test", string lastName = "Usr", string password = PASSWORD, string passwordConfirmation = PASSWORD)
        {
            return new RegisterViewModel
            {
                UserName = userName,
                FirstName = firstName,
                LastName = lastName,
                Password = password,
                PasswordConfirmation = passwordConfirmation
            };
        }


        private LoginViewModel CreateLoginViewModel(string userName = USERNAME, string password = PASSWORD)
        {
            return new LoginViewModel
            {
                UserName = userName,
                Password = password,
                StaySignedIn = false
            };
        }
    }
}