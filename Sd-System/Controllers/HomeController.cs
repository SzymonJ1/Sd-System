using Microsoft.AspNetCore.Mvc;
using Sd_System.Data;
using System.Diagnostics;
using Sd_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Sd_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                ViewBag.TicketsCount = await _context.Tickets.CountAsync();
                ViewBag.UsersCount = await _context.Users.CountAsync();
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}