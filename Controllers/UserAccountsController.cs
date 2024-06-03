using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ManagingAccessService.Models.DBContext;
using ManagingAccessService.Models.DbModels;
using System.Security.Cryptography;
using System.Text;

namespace ManagingAccessService.Controllers
{
    public class UserAccountsController : Controller
    {
        private readonly ManagingAccessServiceContext _context;

        public UserAccountsController(ManagingAccessServiceContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var managingAccessServiceContext = _context.UserAccounts.Include(u => u.Employee).Include(u => u.Role);
            return View(await managingAccessServiceContext.ToListAsync());
        }

        //public IActionResult Create()
        //{
        //    ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
        //    ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId");
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("AccountId,EmployeeId,Email,Login,Password,LastLogin,LastPasswordChange,Status,RoleId")] UserAccount userAccount)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(userAccount);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", userAccount.EmployeeId);
        //    ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", userAccount.RoleId);
        //    return View(userAccount);
        //}
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAccount = await _context.UserAccounts
                .Include(u => u.Employee)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (userAccount == null)
            {
                return NotFound();
            }

            return View(userAccount);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userAccount = await _context.UserAccounts.FindAsync(id);
            if (userAccount != null)
            {
                _context.UserAccounts.Remove(userAccount);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserAccountExists(int id)
        {
            return _context.UserAccounts.Any(e => e.AccountId == id);
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

                // Хеширование пароля перед сохранением
                userAccount.Password = EncryptPassword(userAccount.Password);
                if (userAccount.RoleId != null)
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

                return RedirectToAction("Index");
            }

            // Если модель не прошла валидацию, возвращаем форму снова
            return View(userAccount);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAccount = await _context.UserAccounts.FindAsync(id);
            if (userAccount == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", userAccount.EmployeeId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", userAccount.RoleId);
            return View(userAccount);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,EmployeeId,Email,Login,Password,LastLogin,LastPasswordChange,Status,RoleId")] UserAccount userAccount)
        {
            if (id != userAccount.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userAccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserAccountExists(userAccount.AccountId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", userAccount.EmployeeId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", userAccount.RoleId);
            return View(userAccount);
        }
        [HttpGet]
        public async Task<IActionResult> EditUserAccount(int id)
        {
            // Получение учетной записи пользователя по ID
            var userAccount = await _context.UserAccounts.FindAsync(id);
            if (userAccount == null)
            {
                return NotFound();
            }

            // Преобразование списка сотрудников в SelectListItem
            var employees = _context.Employees.Select(e => new SelectListItem
            {
                Value = e.EmployeeId.ToString(),
                Text = e.FullName 
            }).ToList();
            ViewBag.Employees = employees;

            // Преобразование списка ролей в SelectListItem
            var roles = _context.Roles.Select(r => new SelectListItem
            {
                Value = r.RoleId.ToString(),
                Text = r.Name
            }).ToList();
            ViewBag.Roles = roles;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditUserAccount(UserAccount userAccount)
        {
            if (ModelState.IsValid)
            {
                // Получаем текущую учетную запись из контекста
                var currentAccount = await _context.UserAccounts.FindAsync(userAccount.AccountId);
                if (currentAccount == null)
                {
                    return NotFound();
                }

                // Проверка на существование учетной записи с таким же логином, но исключая текущую учетную запись
                if (currentAccount.Login != userAccount.Login)
                {
                    var existingUser = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Login == userAccount.Login);
                    if (existingUser != null)
                    {
                        // Если учетная запись с таким логином уже существует, добавляем ошибку в модель состояния
                        ModelState.AddModelError("Login", "Учетная запись с таким логином уже существует.");

                        // Преобразование списка сотрудников в SelectListItem
                        var employees = _context.Employees.Select(e => new SelectListItem
                        {
                            Value = e.EmployeeId.ToString(),
                            Text = e.FullName
                        }).ToList();
                        ViewBag.Employees = employees;

                        // Преобразование списка ролей в SelectListItem
                        var roles = _context.Roles.Select(r => new SelectListItem
                        {
                            Value = r.RoleId.ToString(),
                            Text = r.Name
                        }).ToList();
                        ViewBag.Roles = roles;

                        // Возвращаем представление с ошибкой
                        return View(userAccount);
                    }
                }

                // Обновляем поля учетной записи
                currentAccount.Login = userAccount.Login;
                currentAccount.Email = userAccount.Email;
                currentAccount.EmployeeId = userAccount.EmployeeId;
                currentAccount.RoleId = userAccount.RoleId;
                currentAccount.Status = userAccount.Status;

                // Если пароль изменен, хешируем новый пароль
                if (!string.IsNullOrEmpty(userAccount.Password))
                {
                    currentAccount.Password = EncryptPassword(userAccount.Password);
                }

                // Привязка учетной записи к выбранной роли и сотруднику
                if (userAccount.RoleId != 0)
                {
                    var role = _context.Roles.FirstOrDefault(q => q.RoleId == userAccount.RoleId);
                    if (role != null)
                    {
                        currentAccount.Role = role;
                    }
                    else
                    {
                        throw new Exception($"Ошибка создания роли, роль {userAccount.RoleId} не существует");
                    }
                }
                if (userAccount.EmployeeId != null)
                {
                    currentAccount.Employee = await _context.Employees.FindAsync(userAccount.EmployeeId);
                }

                // Сохранение изменений в базе данных
                _context.UserAccounts.Update(currentAccount);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            // Если модель не прошла валидацию, возвращаем форму снова
            return View(userAccount);
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
    }
}
