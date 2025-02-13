using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sd_System.Data;
using Sd_System.Models;

namespace Sd_System.Controllers
{
    [Authorize]
    public class UserTicketsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserTicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserTickets
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tickets = await _context.Tickets
                .Where(t => t.CreatedById == userId)
                .ToListAsync();

            return View(tickets);
        }

        // GET: UserTickets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserTickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description")] Ticket ticket)
        {
            // Wyłącz walidację dla pól CreatedById i CreatedBy
            ModelState.Remove("CreatedById");
            ModelState.Remove("CreatedBy");

            if (ModelState.IsValid)
            {
                try
                {
                    // Ręczne przypisanie wartości
                    ticket.CreatedById = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    ticket.CreatedDate = DateTime.Now;
                    ticket.Status = TicketStatus.New;
                    ticket.Priority = TicketPriority.P5;

                    _context.Add(ticket);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Błąd zapisu: {ex.Message}");
                }
            }

            return View(ticket);
        }
    }
}