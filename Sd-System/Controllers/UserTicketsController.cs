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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                try
                {
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

            // Logowanie błędów
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"Błąd modelu: {error.ErrorMessage}");
            }

            return View(ticket);
        }
    }
}