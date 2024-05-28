using ManagingAccessService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ManagingAccessService.Models.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ManagingAccessService.Models.DbModels;
using Microsoft.AspNetCore.Authorization;

namespace Lab1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ManagingAccessServiceContext _context;
        public HomeController(ManagingAccessServiceContext context)
        {
            _context = context;
            if(User != null)
            {
                CheckRole();
            }
        }
        public void CheckRole()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = _context.UserAccounts.FirstOrDefault(u => u.AccountId == Convert.ToInt32(userId));

            string userRole = "";
            if (user != null)
            {
                userRole = user.Role.Name;
            }

            ViewBag.UserRole = userRole;
        }
        public async Task<IActionResult> Employee()
        {
            var emp = await _context.Employees.ToListAsync();
            return View(emp);
        }
        [HttpGet]
        public async Task<IActionResult> Employees()
        {
            var employees = await _context.Employees
                                             .Include(e => e.ContactInformations)
                                             .ToListAsync();
            return View(employees);
        }

        [HttpGet]
        public async Task<IActionResult> Organizations()
        {
            var organizations = await _context.Organizations
                                                .ToListAsync();
            return View(organizations);
        }

        [HttpGet]
        public async Task<IActionResult> UserAccounts()
        {
            var userAccounts = await _context.UserAccounts.ToListAsync();
            foreach (var userAccount in userAccounts)
            {
                if (userAccount.EmployeeId != null)
                {
                    var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == userAccount.EmployeeId);
                    userAccount.Employee = employee;
                }
            }
            return View(userAccounts);
        }
        [HttpGet]
        public IActionResult AddEmployee()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddEmployee(Employee employee)
        {
            if (ModelState.IsValid)
            {
                // Получение максимального значения EmployeeId из базы данных
                var maxEmployeeId = await _context.Employees.MaxAsync(e => (int?)e.EmployeeId) ?? 0;
                // Увеличение значения на 1 для нового сотрудника
                employee.EmployeeId = maxEmployeeId + 1;

                // Добавление сотрудника в контекст базы данных
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                // Получение обновленного списка сотрудников, отсортированного по EmployeeId
                var employees = await _context.Employees
                                                 .Include(e => e.ContactInformations)
                                                 .OrderBy(e => e.EmployeeId)
                                                 .ToListAsync();
                // Передача обновленного списка в представление
                return View("Employees", employees);
            }
            // Если модель не прошла валидацию, возвращаем форму снова
            return View(employee);
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
