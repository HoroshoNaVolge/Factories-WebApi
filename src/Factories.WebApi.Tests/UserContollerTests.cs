using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Factories.WebApi.BLL.Controllers;
using Factories.WebApi.BLL.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Factories.WebApi.BLL.Services;
using Factories.WebApi.BLL.Authentication;

namespace Factories.WebApi.Tests
{
    [TestFixture]
    public class UserControllerTests
    {
        private Mock<UserManager<IdentityUser>> userManagerMock;
        private Mock<RoleManager<IdentityRole>> roleManagerMock;
        private Mock<IJwtService> jwtServiceMock;
        private UserController controller;

        [SetUp]
        public void Setup()
        {
            userManagerMock = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);
            jwtServiceMock = new Mock<IJwtService>();

            controller = new UserController(jwtServiceMock.Object, userManagerMock.Object, roleManagerMock.Object);
        }

        [Test]
        public async Task Login_ReturnsToken_ForValidUserCredentials()
        {
            var model = new LoginModel { Username = "user@mail.ru", Password = "Password123!" };
            userManagerMock.Setup(x => x.FindByNameAsync(model.Username))
                .ReturnsAsync(new IdentityUser { UserName = model.Username });
            userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<IdentityUser>(), model.Password))
                .ReturnsAsync(true);
            jwtServiceMock.Setup(s => s.GenerateJwtToken(It.IsAny<IdentityUser>(), It.IsAny<IList<Claim>>(), It.IsAny<IList<string>>()))
                .Returns("mocked-token");

            var result = await controller.Login(model);
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null, "Expected OkObjectResult");

            var statusCode = okResult.StatusCode;
            Assert.That(statusCode, Is.EqualTo(200));

            var loginResponse = okResult.Value as LoginResponse;
            Assert.That(loginResponse, Is.Not.Null, "Expected not empty LoginResponse");
            Assert.That(string.IsNullOrEmpty(loginResponse.Token), Is.False, "Token must not be null or empty string");
        }

        [Test]
        public async Task Login_ReturnsNotFound_ForInvalidUsername()
        {
            var model = new LoginModel { Username = "not_existing_user@mail.ru", Password = "AnyPassword" };
            userManagerMock.Setup(x => x.FindByNameAsync(model.Username))
                .ReturnsAsync(null as IdentityUser);

            var result = await controller.Login(model);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>(), "Expected NotFoundObjectResult");

            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult?.StatusCode, Is.EqualTo(404));

            var message = notFoundResult.Value as string;
            Assert.That(message, Is.EqualTo("User not found"));
        }

        [Test]
        public async Task Login_ReturnsNotFound_ForInvalidUserPassword()
        {
            var model = new LoginModel { Username = "correctUser@mail.ru", Password = "WrongPassword" };

            userManagerMock.Setup(x => x.FindByNameAsync(model.Username))
                .ReturnsAsync(new IdentityUser() { UserName = model.Username });

            userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<IdentityUser>(), model.Password))
                .ReturnsAsync(false);

            var result = await controller.Login(model);

            Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>(), "Expected UnauthorizedObjectResult");

            var notFoundResult = result as UnauthorizedObjectResult;
            Assert.That(notFoundResult?.StatusCode, Is.EqualTo(401));

            var message = notFoundResult.Value as string;
            Assert.That(message, Is.EqualTo("Incorrect password"));
        }

    }
}
