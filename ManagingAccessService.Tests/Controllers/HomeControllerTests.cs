using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Lab1.Controllers;
using ManagingAccessService.Models;
using ManagingAccessService.Models.DBContext;
using ManagingAccessService.Models.DbModels;
using ManagingAccessService.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

public class HomeControllerTests
{
    private readonly Mock<ManagingAccessServiceContext> _contextMock;
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        _contextMock = new Mock<ManagingAccessServiceContext>();
        _controller = new HomeController(_contextMock.Object);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        };

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [Fact]
    public void CheckRole_UserHasRole_SetsUserRoleInViewBag()
    {
        // Arrange
        var user = new UserAccount { AccountId = 1, RoleId = 1 };
        var role = new Role { RoleId = 1, Name = "Admin" };

        _contextMock.Setup(c => c.UserAccounts).Returns(CreateMockDbSet(new[] { user }).Object);
        _contextMock.Setup(c => c.Roles).Returns(CreateMockDbSet(new[] { role }).Object);

        // Act
        _controller.CheckRole();

        // Assert
        Assert.Equal("Admin", _controller.ViewBag.UserRole);
    }

    [Fact]
    public async Task Index_UserExists_ReturnsViewResultWithViewModel()
    {
        // Arrange
        var user = new UserAccount { AccountId = 1, EmployeeId = 1 };
        var employee = new Employee { EmployeeId = 1 };

        _contextMock.Setup(c => c.UserAccounts).Returns(CreateMockDbSet(new[] { user }).Object);
        _contextMock.Setup(c => c.Employees).Returns(CreateMockDbSet(new[] { employee }).Object);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<HomeViewModel>(viewResult.Model);
        Assert.Equal(user, model.Account);
        Assert.Equal(employee, model.Employee);
    }

    private static Mock<DbSet<T>> CreateMockDbSet<T>(IEnumerable<T> elements) where T : class
    {
        var queryable = elements.AsQueryable();
        var mockSet = new Mock<DbSet<T>>();

        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

        mockSet.As<IAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<T>(queryable.GetEnumerator()));

        return mockSet;
    }

    private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(_inner.MoveNext());
        }

        public T Current => _inner.Current;
    }
}
