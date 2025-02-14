using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sd_System.Data;
using Sd_System.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Sd_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(
            ApplicationDbContext context,
            ILogger<TicketsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            var activeTickets = await _context.Tickets
         .Include(t => t.CreatedBy)
         .Where(t => t.Status != TicketStatus.Closed)
         .ToListAsync();

            return View(activeTickets);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Tickets
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null) return NotFound();

            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
    [Bind("Id,Title,Description,Status,Priority,CreatedDate,CreatedById")] Ticket ticket, string action)
        {
            if (id != ticket.Id) return NotFound();


            if (string.IsNullOrEmpty(ticket.Title) ||
                string.IsNullOrEmpty(ticket.Description) ||
                ticket.CreatedById == null)
            {
                ModelState.AddModelError("", "Wymagane pola są puste");
                return View(ticket);
            }

            if (action == "archive")
            {
                var existingTicket = await _context.Tickets.FindAsync(id);
                existingTicket.Status = TicketStatus.Closed;
                existingTicket.DueDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var existingTicket = await _context.Tickets.FindAsync(id);
                if (existingTicket == null) return NotFound();


                existingTicket.Status = ticket.Status;
                existingTicket.Priority = ticket.Priority;


                existingTicket.DueDate = ticket.Priority != TicketPriority.P5
                    ? existingTicket.CreatedDate.AddHours((int)ticket.Priority)
                    : null;

                _context.Update(existingTicket);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(ticket.Id)) return NotFound();
                throw;
            }
        }
        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Archive()
        {
            var archivedTickets = await _context.Tickets
                .Include(t => t.CreatedBy)
                .Where(t => t.Status == TicketStatus.Closed)
                .ToListAsync();

            return View(archivedTickets);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.CreatedBy) 
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            _context.Tickets.Remove(ticket); 
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.CreatedBy) 
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            
            ViewBag.TimeLeft = ticket.DueDate.HasValue
                ? (TimeSpan?)(ticket.DueDate.Value - DateTime.Now)
                : null;

            return View(ticket);
        }
        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }
    }
}