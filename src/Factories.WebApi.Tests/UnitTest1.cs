using Factories.WebApi.BLL.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Collections.Generic;
using System;
using Factories.WebApi.BLL.Models;
using Factories.WebApi.BLL.Services;

namespace Factories.WebApi.Tests
{
    public class UserControllerTests
    {
        [Test]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var jwtServiceMock = new Mock<JwtService>("issuer", "audience", "key");
            var userManagerMock = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            var user = new IdentityUser { UserName = "admin" };
            userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            userManagerMock.Setup(x => x.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(true);
            userManagerMock.Setup(x => x.GetClaimsAsync(user)).ReturnsAsync(new List<Claim>());
            userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string>());

            var controller = new UserController(jwtServiceMock.Object, userManagerMock.Object, null);

            var model = new LoginModel { Username = "admin", Password = "p@ssw0rd" };

            // Act
            var result = await controller.Login(model) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.IsInstanceOf<string>(result.Value);
        }

        [Test]
        public async Task Login_WithInvalidCredentials_ReturnsNotFound()
        {
            // Arrange
            var jwtServiceMock = new Mock<JwtService>("issuer", "audience", "key");
            var userManagerMock = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser)null);

            var controller = new UserController(jwtServiceMock.Object, userManagerMock.Object, null);

            var model = new LoginModel { Username = "invaliduser", Password = "invalidpassword" };

            // Act
            var result = await controller.Login(model) as NotFoundObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("User login or password incorrect", result.Value);
        }
    }
}