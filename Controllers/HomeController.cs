using ManagingAccessService.Models;
using Microsoft.AspNetCore.Mvc;
using ManagingAccessService.Models.DBContext;
using Microsoft.EntityFrameworkCore;

namespace Lab1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ManagingAccessServiceContext _context;
        public HomeController(ManagingAccessServiceContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Employee()
        {
            var emp = await _context.Employees.ToListAsync();
            return View(emp);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
