using ManagingAccessService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ManagingAccessService.Models.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ManagingAccessService.Models.DbModels;
using Microsoft.AspNetCore.Authorization;
using ManagingAccessService.Controllers;
using System.Security.Cryptography;
using System.Text;
using ManagingAccessService.ViewModels;

namespace Lab1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ManagingAccessServiceContext _context;
        public HomeController(ManagingAccessServiceContext context)
        {
            _context = context;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            // Получаем текущий пользовательский идентификатор
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return NotFound();
            }
            var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.AccountId.ToString() == userId);
            var emp = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == user.EmployeeId);

            var vm = new HomeViewModel()
            {
                Employee = emp,
                Account = user
            };


            return View(vm);
        }
    }
}
