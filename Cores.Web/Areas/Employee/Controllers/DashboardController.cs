using System.Security.Claims;
using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.Employee;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cores.Web.Areas.Employee.Controllers;

[Area("Employee")]
[Authorize]
public class DashboardController : Controller
{
   private readonly IUnitOfWork _unitOfWork;
   private readonly ApplicationDbContext _db;

   public DashboardController(IUnitOfWork unitOfWork, ApplicationDbContext db)
   {
       _unitOfWork = unitOfWork;
       _db = db;
   }

   public async Task<IActionResult> Index()
   {
       var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
       
       if (userId is null)
           return NotFound();
       
       var employee = await _unitOfWork.ApplicationUser.Get(e => e.Id == userId);
       
       if(employee is null)
           return NotFound();

       var dashboard = new EmployeeDashboard
       {
           EmployeeId = userId,
           CurrentSalary = employee.Salary,
           TotalBenefits = await GetTotalBenefits(userId),
           LeaveBalance = employee.AnnualLeaveBalance,
           PendingRequests = await GetPendingRequests(userId),
           RejectedRequests = await GetRejectedRequests(userId),
           ApprovedRequests = await GetApprovedRequests(userId),
           TotalDeductions =  await GetTotalDeductions(userId),
           MonthlyEarnings = await GetMonthlyEarnings(userId),
           MonthlyRequests = await GetMonthlyRequests(userId)
       };

       return View(dashboard);
   }

   private async Task<decimal?> GetTotalBenefits(string userId)
   {
       return await _db.EmployeeBenefits
           .Where(eb => eb.EmployeeId == userId && 
                       (eb.Status == "Active" || eb.Status == "One-time"))
           .Include(eb => eb.Benefit)
           .SumAsync(eb => eb.Benefit.Amount);
   }
   
   private async Task<decimal?> GetTotalDeductions(string userId)
   {
       return await _db.EmployeeDeductions
           .Where(eb => eb.EmployeeId == userId && 
                        (eb.Status == "Active" || eb.Status == "One-time"))
           .Include(eb => eb.Deduction)
           .SumAsync(eb => eb.Deduction.Amount);
   }

   private async Task<int> GetPendingRequests(string userId)
   {
       return await _db.LeaveRequests.CountAsync(r => r.EmployeeId == userId && r.LeaveStatus == "Pending") +
              await _db.DocumentRequests.CountAsync(r => r.ApplicationUserId == userId && r.Status == "Pending") +
              await _db.BenefitsRequests.CountAsync(r => r.ApplicationUserId == userId && r.Status == "Pending") +
              await _db.CompensationRequests.CountAsync(r => r.ApplicationUserId == userId && r.Status == "Pending") +
              await _db.AdministrativeRequests.CountAsync(r => r.ApplicationUserId == userId && r.Status == "Pending");
   }

   private async Task<int> GetApprovedRequests(string userId) 
   {
       return await _db.LeaveRequests.CountAsync(r => r.EmployeeId == userId && r.LeaveStatus == "Approved") +
              await _db.DocumentRequests.CountAsync(r => r.ApplicationUserId == userId && r.Status == "Approved") +
              await _db.BenefitsRequests.CountAsync(r => r.ApplicationUserId == userId && r.Status == "Approved") +
              await _db.CompensationRequests.CountAsync(r => r.ApplicationUserId == userId && r.Status == "Approved") +
              await _db.AdministrativeRequests.CountAsync(r => r.ApplicationUserId == userId && r.Status == "Approved");
   }

   private async Task<int> GetRejectedRequests(string userId)
   {
       return await _db.LeaveRequests.CountAsync(r => r.EmployeeId == userId && r.LeaveStatus == "Rejected") +
              await _db.DocumentRequests.CountAsync(r => r.ApplicationUserId == userId && r.Status == "Rejected") +
              await _db.BenefitsRequests.CountAsync(r => r.ApplicationUserId == userId && r.Status == "Rejected") +
              await _db.CompensationRequests.CountAsync(r => r.ApplicationUserId == userId && r.Status == "Rejected") +
              await _db.AdministrativeRequests.CountAsync(r => r.ApplicationUserId == userId && r.Status == "Rejected");
   }

   private async Task<Dictionary<string, decimal?>> GetMonthlyEarnings(string userId)
   {
       var oneYearAgo = DateTime.Now.AddYears(-1);

       var earnings = await _db.Payrolls
           .Where(p => p.EmployeeId == userId && p.PaymentDate >= oneYearAgo)
           .GroupBy(p => new { p.PaymentDate!.Value.Year, p.PaymentDate!.Value.Month })
           .Select(g => new
           {
               YearMonth = new DateTime(g.Key.Year, g.Key.Month, 1),
               Total = g.Sum(p => p.NetPay)
           })
           .ToDictionaryAsync(
               k => k.YearMonth.ToString("MMM"),
               v => v.Total
           );

       var allMonths = new[] 
       { 
           "Jan", "Feb", "Mar", "Apr", "May", "Jun", 
           "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
       };
       
       return allMonths.ToDictionary(
           month => month,
           month => earnings.TryGetValue(month, out var value) ? value : 0m
       );
   }

   private async Task<Dictionary<string, int>> GetMonthlyRequests(string userId)
   {
       var oneYearAgo = DateTime.Now.AddYears(-1);

       var requests = await _db.LeaveRequests
           .Where(r => r.EmployeeId == userId && r.RequestDate >= oneYearAgo)
           .Select(r => new { r.RequestDate })
           .Concat(_db.DocumentRequests
               .Where(r => r.ApplicationUserId == userId && r.RequestDate >= oneYearAgo)
               .Select(r => new { r.RequestDate }))
           .Concat(_db.BenefitsRequests
               .Where(r => r.ApplicationUserId == userId && r.RequestDate >= oneYearAgo)
               .Select(r => new { r.RequestDate }))
           .Concat(_db.CompensationRequests
               .Where(r => r.ApplicationUserId == userId && r.RequestDate >= oneYearAgo)
               .Select(r => new { r.RequestDate }))
           .Concat(_db.AdministrativeRequests
               .Where(r => r.ApplicationUserId == userId && r.RequestDate >= oneYearAgo)
               .Select(r => new { r.RequestDate }))
           .GroupBy(r => new { r.RequestDate.Year, r.RequestDate.Month })
           .Select(g => new
           {
               YearMonth = new DateTime(g.Key.Year, g.Key.Month, 1),
               Count = g.Count()
           })
           .ToDictionaryAsync(
               k => k.YearMonth.ToString("MMM"),
               v => v.Count
           );

       var allMonths = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
       return allMonths.ToDictionary(
           month => month,
           month => requests.TryGetValue(month, out var value) ? value : 0
       );
   }
}