using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using ManagingAccessService.Controllers;
using ManagingAccessService.Models.DBContext;
using ManagingAccessService.Models.DbModels;

namespace ManagingAccessService.Tests
{
    public class EmployeesControllerTests : IDisposable
    {
        private readonly DbContextOptions<ManagingAccessServiceContext> _options;
        private readonly ManagingAccessServiceContext _context;
        private readonly EmployeesController _controller;

        public EmployeesControllerTests()
        {
            // Set up in-memory database options
            _options = new DbContextOptionsBuilder<ManagingAccessServiceContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Create the in-memory database context
            _context = new ManagingAccessServiceContext(_options);

            // Seed the database with some test data
            SeedDatabase();

            // Create the controller
            _controller = new EmployeesController(_context);
        }

        private void SeedDatabase()
        {
            var employees = new List<Employee>
            {
                new Employee {FullName = "John Doe", Gender = "Male", DateOfBirth = new DateOnly(1990, 1, 1), Identifier = "ID001", ContactInformation = "johndoe@example.com", Status = "Active" },
                new Employee {FullName = "Jane Smith", Gender = "Female", DateOfBirth = new DateOnly(1985, 5, 15), Identifier = "ID002", ContactInformation = "janesmith@example.com", Status = "Active" }
            };

            _context.Employees.AddRange(employees);
            _context.SaveChanges();
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithAListOfEmployees()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Employee>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_WhenIdIsNull()
        {
            // Act
            var result = await _controller.Edit(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_WhenEmployeeNotFound()
        {
            // Act
            var result = await _controller.Edit(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsViewResult_WithEmployee()
        {
            // Act
            var result = await _controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Employee>(viewResult.ViewData.Model);
            Assert.Equal(1, model.EmployeeId);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenIdIsNull()
        {
            // Act
            var result = await _controller.Delete(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenEmployeeNotFound()
        {
            // Act
            var result = await _controller.Delete(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task Genocide_DeletesMultipleEmployeesAndRedirects()
        {
            // Arrange
            var idsToDelete = new List<int> { 1, 2 };

            // Act
            var result = await _controller.Genocide(idsToDelete);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Empty(_context.Employees.ToList());
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
