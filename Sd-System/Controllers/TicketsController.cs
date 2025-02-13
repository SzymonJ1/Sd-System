using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sd_System.Data;
using Sd_System.Models;

namespace Sd_System.Controllers
{

    [Authorize(Roles = "Admin")]
    public class TicketsController : Controller
    {
        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }

        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ... inne metody ...

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Status,Priority")] Ticket ticket)
        {
            if (id != ticket.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingTicket = await _context.Tickets
                        .Include(t => t.CreatedBy)
                        .FirstOrDefaultAsync(t => t.Id == id);

                    existingTicket.Title = ticket.Title;
                    existingTicket.Description = ticket.Description;
                    existingTicket.Status = ticket.Status;
                    existingTicket.Priority = ticket.Priority;

                    existingTicket.DueDate = ticket.Priority switch
                    {
                        TicketPriority.P1 => existingTicket.CreatedDate.AddHours((int)TicketPriority.P1),
                        TicketPriority.P2 => existingTicket.CreatedDate.AddHours((int)TicketPriority.P2),
                        TicketPriority.P3 => existingTicket.CreatedDate.AddHours((int)TicketPriority.P3),
                        TicketPriority.P4 => existingTicket.CreatedDate.AddHours((int)TicketPriority.P4),
                        _ => null
                    };

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // ... reszta metod ...
    }
}