using System.Security.Claims;
using Cores.Models;
using Cores.Models.Accounting;
using Cores.Models.CRM;
using Cores.Models.HR;
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
                "Cores.Models.Notification",
                "Cores.Models.CRM.Order",
                "Cores.Models.ApplicationUser", 
                "Microsoft.AspNetCore.Identity.IdentityUserRole`1[System.String]", 
                "System.Collections.Generic.Dictionary`2[System.String,System.Object]"
    ];

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<MessagePayload> MessagePayloads { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Problem> Problems { get; set; }
    public DbSet<ProblemType> ProblemTypes { get; set; }
    public DbSet<EventType> EventTypes { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Todo> Todos { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<LeaveType> LeaveTypes { get; set; }
    public DbSet<LeaveRequest> LeaveRequests { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<Recruitment> Recruitments { get; set; }
    public DbSet<JobApplication> JobApplications { get; set; }
    public DbSet<Salary> Salaries { get; set; }
    public DbSet<Benefit> Benefits { get; set; }
    public DbSet<Deduction> Deductions { get; set; }
    public DbSet<EmployeeBenefit> EmployeeBenefits { get; set; }
    public DbSet<EmployeeDeduction> EmployeeDeductions { get; set; }
    public DbSet<Archive> Archives { get; set; }
    public DbSet<ArchiveType> ArchiveTypes { get; set; }
    public DbSet<Training> Trainings { get; set; }
    public DbSet<EmployeeTraining> EmployeeTrainings { get; set; }
    public DbSet<PerformanceReview> PerformanceReviews { get; set; }
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<TransactionDetail> TransactionDetails { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Journal> Journals { get; set; }
    public DbSet<JournalEntry> JournalEntries { get; set; }
    public DbSet<JournalEntryDetail> JournalEntryDetails { get; set; }
    public DbSet<Tax> Taxes { get; set; }
    public DbSet<Payroll> Payrolls { get; set; }
    public DbSet<WorkSchedule> WorkSchedules { get; set; }
    public DbSet<EmployeeLeaveBalance> EmployeeLeaveBalances { get; set; }
    public DbSet<UnpaidLeaveDeduction> UnpaidLeaveDeductions { get; set; }
    public DbSet<HolidayType> HolidayTypes { get; set; }
    public DbSet<Holiday> Holidays { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
        IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }


    /*
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       
        base.OnModelCreating(modelBuilder);
    }
    */

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
{
    var modifiedEntries = ChangeTracker
        .Entries()
        .Where(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
        .ToList();

    var httpContext = _httpContextAccessor.HttpContext;
    if (httpContext != null)
    {
        var claimsIdentity = httpContext.User.Identity as ClaimsIdentity;
        var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        foreach (var entity in modifiedEntries)
        {
            if (_entitiesToSkip.Contains(entity.Entity.GetType().ToString())) 
                continue;
            var updates = GetUpdate(entity);
            if (updates.Count == 0)
                updates.Add(new List<string?> { "", "", "" });

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
    }
    else
    {
        Console.WriteLine("No HTTP context available. Skipping audit log.");
    }

    return await base.SaveChangesAsync(cancellationToken);
}

    private static List<List<string?>> GetUpdate(EntityEntry entry)
    {
        var list = new List<List<string?>>();
        foreach (var prop in entry.OriginalValues.Properties)
        {
            var originalValue = entry.OriginalValues[prop];
            var currentValue = entry.CurrentValues[prop];
            if (currentValue is null || originalValue is null) continue;
            if (!Equals(originalValue, currentValue))
                list.Add(new List<string?> { prop.Name, originalValue.ToString(), currentValue.ToString() });
        }

        return list;
    }
}