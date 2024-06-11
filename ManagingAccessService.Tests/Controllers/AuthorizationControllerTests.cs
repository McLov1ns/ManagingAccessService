using ManagingAccessService.Controllers;
using ManagingAccessService.Models;
using ManagingAccessService.Models.DBContext;
using ManagingAccessService.Models.DbModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ManagingAccessService.Tests
{
    public class AuthorizationControllerTests
    {
        private readonly AuthorizationController _controller;
        private readonly ManagingAccessServiceContext _context;

        public AuthorizationControllerTests()
        {
            var options = new DbContextOptionsBuilder<ManagingAccessServiceContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ManagingAccessServiceContext(options);
            _controller = new AuthorizationController(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            // Add test data to the in-memory database
            var userRole = new Role { RoleId = 1, Name = "Пользователь" };
            _context.Roles.Add(userRole);
            _context.SaveChanges();
        }

        [Fact]
        public async Task Login_Post_ReturnsViewResultWithError_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Login", "Required");
            var model = new UserAccount();

            // Act
            var result = await _controller.Login(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
        }

        [Fact]
        public async Task Login_Post_ReturnsViewResultWithErrorMessage_WhenUserNotFound()
        {
            // Arrange
            var model = new UserAccount { Login = "nonexistent", Password = "password" };

            // Act
            var result = await _controller.Login(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Неверный логин или пароль", viewResult.ViewData["ErrorMessage"]);
        }

        [Fact]
        public void Registration_Post_ReturnsViewResultWithErrorMessage_WhenUserAlreadyExists()
        {
            // Arrange
            var existingUser = new UserAccount { Login = "existingUser", Password = "password" };
            _context.UserAccounts.Add(existingUser);
            _context.SaveChanges();

            var model = new UserAccount { Login = "existingUser", Password = "password" };

            // Act
            var result = _controller.Registration(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Пользователь с таким логином уже существует.", viewResult.ViewData["ErrorMessage"]);
        }

        [Fact]
        public void Registration_Post_ReturnsRedirectToActionResult_WhenModelIsValid()
        {
            // Arrange
            var model = new UserAccount { Login = "newUser", Password = "password" };

            // Act
            var result = _controller.Registration(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectToActionResult.ActionName);
        }
    }
}
