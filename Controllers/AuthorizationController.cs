using ManagingAccessService.Models;
using ManagingAccessService.Models.DBContext;
using ManagingAccessService.Models.DbModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace ManagingAccessService.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly ManagingAccessServiceContext _context;

        public AuthorizationController(ManagingAccessServiceContext context)
        {
            _context = context;
            _context.UserAccounts.Load();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserAccount model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.UserAccounts.FirstOrDefault(u => u.Login == model.Login);

                if (user != null)
                {
                    
                    var encpass = EncryptPassword(model.Password!);
                    if (encpass == user.Password)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, model.Login!),
                            new Claim(ClaimTypes.NameIdentifier, user.AccountId.ToString())
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                        return RedirectToAction("Index", "Home");
                    }
                }
                ViewBag.ErrorMessage = "Неверный логин или пароль";
            }

            return View(model);
        }
        public async Task<IActionResult> Exit()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
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

        [HttpGet]

        public IActionResult Registration()
        {
            return View();

        }
        [HttpPost]
        public IActionResult Registration(UserAccount model, Role rol)
        {
            if (ModelState.IsValid)
            {
                var existingAccount = _context.UserAccounts.FirstOrDefault(a => a.Login == model.Login);
                if (existingAccount != null)
                {
                    ViewBag.ErrorMessage = "Пользователь с таким логином уже существует.";
                    return View(model);
                }
                var account = new UserAccount
                {
                    LastLogin = DateOnly.FromDateTime(DateTime.Today),
                    Login = model.Login,
                    Password = EncryptPassword(model.Password!),
                    RoleId = 2
                };
                var role = new Role
                {
                    Name = rol.Name
                };

                _context.UserAccounts.Add(account);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }
            return View("Registration", model);
        }
    }
}
