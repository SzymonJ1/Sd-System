using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sd_System.Data;
using Sd_System.Models;

namespace Sd_System.Controllers
{
    [Authorize] // Tylko zalogowani użytkownicy mogą korzystać z tego kontrolera
    public class UserTicketsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserTicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserTickets/Index
        public async Task<IActionResult> Index()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Pobierz ID zalogowanego użytkownika
            var userTickets = await _context.Tickets
                .Where(t => t.CreatedById == currentUserId) // Filtruj zgłoszenia tylko dla danego użytkownika
                .ToListAsync();

            return View(userTickets); // Przekaż listę zgłoszeń do widoku
        }

        // GET: UserTickets/Create
        public IActionResult Create()
        {
            return View(); // Zwróć widok do tworzenia nowego zgłoszenia
        }

        // POST: UserTickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ticket.CreatedById = User.FindFirstValue(ClaimTypes.NameIdentifier); // Przypisz zgłoszenie do zalogowanego użytkownika
                    ticket.CreatedDate = DateTime.Now;
                    ticket.Status = TicketStatus.New;
                    ticket.Priority = TicketPriority.P5;

                    _context.Add(ticket);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index)); // Przekieruj do listy zgłoszeń po utworzeniu
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

            return View(ticket); // W przypadku błędu zwróć widok z formularzem
        }
    }
}