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
using Microsoft.CodeAnalysis.Elfie.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace ManagingAccessService.Controllers
{
    public class UserAccountsController : Controller
    {
        private readonly ManagingAccessServiceContext _context;

        public UserAccountsController(ManagingAccessServiceContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Index()
        {
            var managingAccessServiceContext = _context.UserAccounts.Include(u => u.Employee).Include(u => u.Role);
            return View(await managingAccessServiceContext.ToListAsync());
        }
        [Authorize(Roles = "Администратор")]
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
        [Authorize(Roles = "Администратор")]
        [HttpGet]
        public IActionResult Create()
        {
            var employees = _context.Employees.ToList();
            ViewBag.Employees = employees;

            var roles = _context.Roles.Select(r => new SelectListItem
            {
                Value = r.RoleId.ToString(),
                Text = r.Name
            }).ToList();

            ViewBag.Roles = roles;
            return View();
        }
        [Authorize(Roles = "Администратор")]
        [HttpPost]
        public async Task<IActionResult> Create(UserAccount userAccount)
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
                userAccount.LastLogin = DateOnly.FromDateTime(DateTime.Now);

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

        [Authorize(Roles = "Администратор")]
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
            var roles = _context.Roles.Select(r => new SelectListItem
            {
                Value = r.RoleId.ToString(),
                Text = r.Name
            }).ToList();
            ViewBag.Roles = roles;

            var employees = _context.Employees.ToList();
            ViewBag.Employees = employees;
            return View(userAccount);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,Email,EmployeeId,Login,RoleId,LastLogin,Status")] UserAccount userAccount)
        {
            if (id != userAccount.AccountId)
            {
                return NotFound();
            }
            var roles = _context.Roles.Select(r => new SelectListItem
            {
                Value = r.RoleId.ToString(),
                Text = r.Name
            }).ToList();
            ViewBag.Roles = roles;

            var employees = _context.Employees.ToList();
            ViewBag.Employees = employees;
            UserAccount user = _context.UserAccounts.Find(userAccount.AccountId);
            if (user != null)
            {
                if (userAccount.Login != null)
                    user.Login = userAccount.Login;
                if(userAccount.Password != null)
                    user.Password = userAccount.Password;
                if (userAccount.Password != null)
                    user.Password = userAccount.Password;
                if (userAccount.Email != null)
                    user.Email = userAccount.Email;
                if(userAccount.RoleId !=0)
                    user.RoleId = userAccount.RoleId;
                if (userAccount.EmployeeId != 0)
                    user.EmployeeId = userAccount.EmployeeId;
                if (userAccount.Status != null)
                    user.Status = userAccount.Status;
                    user.LastLogin = DateOnly.FromDateTime(DateTime.Now);

            }
            
            var check = ModelState.FirstOrDefault(q => q.Key == "Login");
            if (check.Value != null)
            {
                try
                {
                    _context.Update(user);
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
