using Microsoft.AspNetCore.Identity;

namespace Sd_System.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Dodatkowe pola np. imię i nazwisko
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
