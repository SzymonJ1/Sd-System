using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sd_System.Models;

namespace Sd_System.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Stwórz role "Admin" i "User", jeśli nie istnieją
                string[] roles = { "Admin", "User" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                // Stwórz konto admina, jeśli nie istnieje
                var adminEmail = "admin@example.com";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FirstName = "Admin",
                        LastName = "System",
                        EmailConfirmed = true // Pominięcie potwierdzenia email
                    };

                    // Hasło musi spełniać wymagania (np. duże litery, cyfry)
                    var result = await userManager.CreateAsync(adminUser, "Admin123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                    else
                    {
                        // Logowanie błędów (np. do konsoli)
                        Console.WriteLine("Błąd podczas tworzenia admina:");
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine(error.Description);
                        }
                    }
                }

                // Zastosuj migracje (na wypadek, gdyby baza nie istniała)
                await context.Database.MigrateAsync();
            }
        }
    }
}
