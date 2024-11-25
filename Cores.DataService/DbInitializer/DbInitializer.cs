using Cores.DataService.Data;
using Cores.Models;
using Cores.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cores.DataService.DbInitializer;

public class DbInitializer : IDbInitializer
{

    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _db;

    public DbInitializer(UserManager<IdentityUser> userManager, 
        RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
    }

    public async Task Initialize()
    {
        // Apply migration if they are not applied 
        try
        {
            Console.WriteLine("Starting database initialization...");

            // Check for pending migrations
            var pendingMigrations = await _db.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                Console.WriteLine($"Found {pendingMigrations.Count()} pending migrations. Applying...");
                await _db.Database.MigrateAsync();
                Console.WriteLine("Migrations applied successfully.");
            }
            else
            {
                Console.WriteLine("No pending migrations found.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred during database initialization: {e.Message}");
            Console.WriteLine($"Stack trace: {e.StackTrace}");
        }

        // Create roles if they are not created 
        if (!await _roleManager.RoleExistsAsync(SD.ADMIN_ROLE))
        {
            await _roleManager.CreateAsync(new IdentityRole(SD.ADMIN_ROLE));
            await _roleManager.CreateAsync(new IdentityRole(SD.ACCOUNTING_ROLE));
            await _roleManager.CreateAsync(new IdentityRole(SD.HR_ROLE));
            await _roleManager.CreateAsync(new IdentityRole(SD.CRM_ROLE));
            await _roleManager.CreateAsync(new IdentityRole(SD.EMPLOYEE_ROLE));

            // Check if admin user exists
            var adminUser = await _userManager.FindByEmailAsync("admin@cores.com");
            if (adminUser is null)
            {
                // Create admin user
                adminUser = new ApplicationUser
                {
                    UserName = "admin@cores.com",
                    Email = "admin@cores.com",
                    FirstName = "cores",
                    LastName = "admin",
                    PhoneNumber = "97595234",
                };

                var result = await _userManager.CreateAsync(adminUser, "Abood123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, SD.ADMIN_ROLE);
                }
                else
                {
                    // Log the error
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error creating admin user: {error.Description}");
                    }
                }
            }
        }
    }
}