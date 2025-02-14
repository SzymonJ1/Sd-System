using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sd_System.Data;
using Sd_System.Models;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<IActionResult> Index(
            string sortOrder = "date_desc",
            string priorityFilter = "")
        {
            ViewData["DateSortParam"] = sortOrder == "date_asc" ? "date_desc" : "date_asc";
            ViewData["CurrentPriorityFilter"] = priorityFilter;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var tickets = _context.Tickets
                .Where(t => t.CreatedById == userId)
                .AsQueryable();

            // Filtrowanie
            if (!string.IsNullOrEmpty(priorityFilter))
            {
                var priority = Enum.Parse<TicketPriority>(priorityFilter);
                tickets = tickets.Where(t => t.Priority == priority);
            }

            // Sortowanie
            tickets = sortOrder switch
            {
                "date_asc" => tickets.OrderBy(t => t.CreatedDate),
                "date_desc" => tickets.OrderByDescending(t => t.CreatedDate),
                _ => tickets.OrderByDescending(t => t.CreatedDate)
            };

            return View(await tickets.ToListAsync());
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
            
            ModelState.Remove("CreatedById");
            ModelState.Remove("CreatedBy");

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

            return View(ticket);
        }
    }
}