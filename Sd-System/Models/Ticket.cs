using System.ComponentModel.DataAnnotations.Schema;

namespace Sd_System.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TicketStatus Status { get; set; } = TicketStatus.New;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Priorytet (domyślnie P5 - bezterminowe)
        public TicketPriority Priority { get; set; } = TicketPriority.P5;

        // Termin realizacji (obliczany na podstawie priorytetu)
        public DateTime? DueDate { get; set; }

        public string CreatedById { get; set; } 

        [ForeignKey("CreatedById")]
        public ApplicationUser CreatedBy { get; set; } 
    }

    public enum TicketPriority
    {
        P1 = 24,    // 24h
        P2 = 48,    // 48h
        P3 = 96,    // 96h
        P4 = 240,   // 240h
        P5 = 0      // Bezterminowo
    }

    public enum TicketStatus
    {
        New,
        InProgress,
        Resolved,
        Closed
    }
}
