using System.Security.Claims;
using Cores.Models;
using Cores.Models.Accounting;
using Cores.Models.CRM;
using Cores.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Cores.DataService.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HashSet<string> _entitiesToSkip = [
                "Cores.Models.MessagePayload", 
                "Cores.Models.CheckBox", 
                "Cores.Models.ApplicationUser", 
                "Microsoft.AspNetCore.Identity.IdentityUserRole`1[System.String]", 
                "System.Collections.Generic.Dictionary`2[System.String,System.Object]"
    ];

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<MessagePayload> MessagePayloads { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<Status> Statuses { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Problem> Problems { get; set; }
    public DbSet<ProblemType> ProblemTypes { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
        IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
     
        var modifiedEntries = ChangeTracker
            .Entries()
            .Where(x => x.State is
                EntityState.Added or EntityState.Modified
                or EntityState.Deleted)
            .ToList();
        var claimsIdentity = (ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity!;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        foreach (var entity in modifiedEntries)
        {
            if (_entitiesToSkip.Contains(entity.Entity.GetType().ToString())) 
                continue;
               
            var updates = GetUpdate(entity);
            if (updates.Count == 0)
                updates.Add(["", "", ""]);
            foreach (var update in updates)
            {
                var auditLog = new ActivityLog
                {
                    Action = entity.State.ToString(),
                    TimeStamp = DateTime.Now,
                    EntityType = entity.Entity.GetType().ToString(),
                    ApplicationUserId = userId,
                    Property = updates.Count != 0 ? update[0] : "",
                    OldValue = updates.Count != 0 ? update[1] : "",
                    NewValue = updates.Count != 0 ? update[2] : ""
                };
                ActivityLogs.Add(auditLog);
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    private static List<List<string?>> GetUpdate(EntityEntry entry)
    {
        List<List<string?>> list = [];
        foreach (var prop in entry.OriginalValues.Properties)
        {
            var originalValue = entry.OriginalValues[prop];
            var currentValue = entry.CurrentValues[prop];
            if (currentValue is null || originalValue is null) continue;
            if (!Equals(originalValue, currentValue))
                list.Add([prop.Name, originalValue.ToString(), currentValue.ToString()]);
        }

        return list;
    }
}