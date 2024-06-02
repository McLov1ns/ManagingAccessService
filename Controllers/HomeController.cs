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
            CheckRole();
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
        [HttpGet]
        public IActionResult CreateUserAccount()
        {
            // Получение списка сотрудников для передачи в представление
            var employees = _context.Employees.ToList();
            ViewBag.Employees = employees;

            // Получение списка ролей для передачи в представление
            var roles = _context.Roles.Select(r => new SelectListItem
            {
                Value = r.RoleId.ToString(),
                Text = r.Name
            }).ToList();

            ViewBag.Roles = roles;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAccount(UserAccount userAccount)
        {
            if (ModelState.IsValid)
            {
                // Проверка на существование учетной записи с таким же логином
                var existingUser = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Login == userAccount.Login);
                if (existingUser != null)
                {
                    // Если учетная запись с таким логином уже существует, добавляем ошибку в модель состояния
                    ModelState.AddModelError("Login", "Учетная запись с таким логином уже существует.");

                    // Повторно загружаем список ролей и сотрудников для представления
                    var roles = _context.Roles.Select(r => new SelectListItem
                    {
                        Value = r.RoleId.ToString(),
                        Text = r.Name
                    }).ToList();
                    ViewBag.Roles = roles;

                    var employees = _context.Employees.ToList();
                    ViewBag.Employees = employees;

                    // Возвращаем представление с ошибкой
                    return View(userAccount);
                }

                // Получение максимального значения AccountId из базы данных
                //var maxAccountId = await _context.UserAccounts.MaxAsync(u => (int?)u.AccountId) ?? 0;
                // Увеличение значения на 1 для новой учетной записи
                //userAccount.AccountId = maxAccountId + 1;

                // Хеширование пароля перед сохранением
                userAccount.Password = EncryptPassword(userAccount.Password);
                if(userAccount.RoleId != null)
                {
                    var role = _context.Roles.FirstOrDefault(q => q.RoleId == userAccount.RoleId);
                    if (role != null)
                    {
                        userAccount.Role = role;
                    }
                    else
                    {
                        throw new Exception($"Ошибка создания роли, роль {userAccount.RoleId} не существует");
                    }
                }
                // Привязка учетной записи к выбранному сотруднику
                if (userAccount.EmployeeId != null)
                {
                    userAccount.Employee = await _context.Employees.FindAsync(userAccount.EmployeeId);
                }

                // Добавление учетной записи в контекст базы данных
                _context.UserAccounts.Add(userAccount);
                await _context.SaveChangesAsync();

                return RedirectToAction("UserAccount");
            }

            // Если модель не прошла валидацию, возвращаем форму снова
            return View(userAccount);
        }
        [HttpGet]
        public async Task<IActionResult> UserAccount()
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
        private string EncryptPassword(string password)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha512.ComputeHash(inputBytes);
                string hash = BitConverter.ToString(hashBytes);

                return hash;
            }
        }
        [Authorize]
        public IActionResult Index()
        {
            CheckRole();
            return View();
        }
    }
}
