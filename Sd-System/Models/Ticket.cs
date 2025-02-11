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

        [ForeignKey("CreatedBy")]
        public string CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }
    }

    public enum TicketStatus
    {
        New,
        InProgress,
        Resolved,
        Closed
    }
}
