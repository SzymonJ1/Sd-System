using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sd_System.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tytuł jest wymagany")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Opis jest wymagany")]
        public string Description { get; set; }

        public TicketStatus Status { get; set; } = TicketStatus.New;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Priorytet")]
        public TicketPriority Priority { get; set; } = TicketPriority.P5;

        [Display(Name = "Termin realizacji")]
        public DateTime? DueDate { get; set; }

        public string CreatedById { get; set; }

        [ForeignKey("CreatedById")]
        public ApplicationUser CreatedBy { get; set; }
    }

    public enum TicketPriority
    {
        [Display(Name = "P1 (24h)")]
        P1 = 24,
        [Display(Name = "P2 (48h)")]
        P2 = 48,
        [Display(Name = "P3 (96h)")]
        P3 = 96,
        [Display(Name = "P4 (240h)")]
        P4 = 240,
        [Display(Name = "P5 (bezterminowo)")]
        P5 = 0
    }

    public enum TicketStatus
    {
        [Display(Name = "Nowe")]
        New,
        [Display(Name = "W realizacji")]
        InProgress,
        [Display(Name = "Rozwiązane")]
        Resolved,
        [Display(Name = "Zamknięte")]
        Closed
    }
}