using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagingAccessService.Controllers;
using ManagingAccessService.Models.DBContext;
using ManagingAccessService.Models.DbModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ManagingAccessService.Tests.Controllers
{
    public class OrganizationsControllerTests
    {
        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfOrganizations()
        {
            // Arrange
            var mockContext = new Mock<ManagingAccessServiceContext>();
            var controller = new OrganizationsController(mockContext.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Organization>>(viewResult.ViewData.Model);
            Assert.NotNull(model);
        }

        [Fact]
        public async Task CreatePost_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            var mockContext = new Mock<ManagingAccessServiceContext>();
            var controller = new OrganizationsController(mockContext.Object);
            var organization = new Organization { Name = "Test Organization" };

            // Act
            var result = await controller.Create(organization);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task EditPost_ReturnsNotFoundResult_WhenIdIsNull()
        {
            // Arrange
            var mockContext = new Mock<ManagingAccessServiceContext>();
            var controller = new OrganizationsController(mockContext.Object);
            var organization = new Organization { OrganizationId = 1 };

            // Act
            var result = await controller.Edit(0, organization);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // You can continue adding more tests for other actions in a similar manner
    }
}
