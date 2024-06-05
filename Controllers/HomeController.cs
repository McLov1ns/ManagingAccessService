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
        public void CheckRole()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = _context.UserAccounts.FirstOrDefault(u => u.AccountId == Convert.ToInt32(userId));

            string userRole = "";
            if (user != null)
            {
                userRole = _context.Roles.FirstOrDefault(q => q.RoleId == user.RoleId).Name;
            }

            ViewBag.UserRole = userRole;
        }
        [Authorize]
        public async Task<IActionResult> Vindex()
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
        [Authorize]
        public IActionResult Index()
        {
            CheckRole();
            return View();
        }
    }
}
