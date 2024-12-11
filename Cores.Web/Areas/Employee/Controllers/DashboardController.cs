using System.Security.Claims;
using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Models.Employee;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

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
   
   
   [HttpGet]
   public async Task<IActionResult> GenerateReport()
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


       var pdfData = GenerateReport(dashboard);
       return File(pdfData, "application/pdf", $"Employee_Dashboard_Report_{DateTime.Now:yyyyMMdd}.pdf");
   }

   public static byte[] GenerateReport(EmployeeDashboard dashboard)
   {
       return Document.Create(document =>
       {
           document.Page(page =>
           {
               page.Margin(20);

               // Header
               page.Header().Row(row =>
               {
                   row.RelativeItem().Column(col =>
                   {
                       col.Item().Text("Employee Dashboard Report").Bold().FontSize(24);
                       col.Item().Text($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(12);
                   });
               });

               page.Content().Column(col =>
               {
                   // Employee Overview Section
                   col.Item().PaddingTop(20).Text("Employee Overview").Bold().FontSize(20);
                   col.Item().PaddingTop(10).Table(table =>
                   {
                       table.ColumnsDefinition(columns =>
                       {
                           columns.RelativeColumn();
                           columns.RelativeColumn();
                           columns.RelativeColumn();
                       });

                       // Salary & Benefits
                       table.Cell().Background("#EEF2FF").Padding(10).Column(cell =>
                       {
                           cell.Item().Text("Current Salary").Bold();
                           cell.Item().PaddingTop(5).Text($"{dashboard.CurrentSalary:C}");
                       });

                       table.Cell().Background("#FEF2F2").Padding(10).Column(cell =>
                       {
                           cell.Item().Text("Total Benefits").Bold();
                           cell.Item().PaddingTop(5).Text($"{dashboard.TotalBenefits:C}");
                       });

                       table.Cell().Background("#ECFDF5").Padding(10).Column(cell =>
                       {
                           cell.Item().Text("Total Deductions").Bold();
                           cell.Item().PaddingTop(5).Text($"{dashboard.TotalDeductions:C}");
                       });
                   });

                   // Leave & Requests Overview
                   col.Item().PaddingTop(10).Table(table =>
                   {
                       table.ColumnsDefinition(columns =>
                       {
                           columns.RelativeColumn();
                           columns.RelativeColumn();
                           columns.RelativeColumn();
                           columns.RelativeColumn();
                       });

                       table.Cell().Background("#FEF3C7").Padding(10).Column(cell =>
                       {
                           cell.Item().Text("Leave Balance").Bold();
                           cell.Item().PaddingTop(5).Text($"{dashboard.LeaveBalance:F1} days");
                       });

                       table.Cell().Background("#DCF7E3").Padding(10).Column(cell =>
                       {
                           cell.Item().Text("Approved Requests").Bold();
                           cell.Item().PaddingTop(5).Text(dashboard.ApprovedRequests.ToString());
                       });

                       table.Cell().Background("#FEE2E2").Padding(10).Column(cell =>
                       {
                           cell.Item().Text("Rejected Requests").Bold();
                           cell.Item().PaddingTop(5).Text(dashboard.RejectedRequests.ToString());
                       });

                       table.Cell().Background("#EDE9FE").Padding(10).Column(cell =>
                       {
                           cell.Item().Text("Pending Requests").Bold();
                           cell.Item().PaddingTop(5).Text(dashboard.PendingRequests.ToString());
                       });
                   });

                   // Monthly Earnings Section
                   col.Item().PaddingTop(30).Text("Monthly Earnings Analysis").Bold().FontSize(20);
                   col.Item().PaddingTop(10).Table(table =>
                   {
                       table.ColumnsDefinition(columns =>
                       {
                           columns.RelativeColumn();
                           columns.RelativeColumn();
                       });

                       // Header
                       table.Header(header =>
                       {
                           header.Cell().Background("#D1D5DB").Padding(5).Text("Month").Bold();
                           header.Cell().Background("#D1D5DB").Padding(5).Text("Net Earnings").Bold();
                       });

                       // Content
                       foreach (var month in dashboard.MonthlyEarnings.OrderByDescending(x => x.Key))
                       {
                           table.Cell().Padding(5).Text(month.Key);
                           table.Cell().Padding(5).Text($"{month.Value:C}");
                       }
                   });

                   // Monthly Requests Section
                   col.Item().PaddingTop(30).Text("Monthly Requests Analysis").Bold().FontSize(20);
                   col.Item().PaddingTop(10).Table(table =>
                   {
                       table.ColumnsDefinition(columns =>
                       {
                           columns.RelativeColumn();
                           columns.RelativeColumn();
                       });

                       // Header
                       table.Header(header =>
                       {
                           header.Cell().Background("#D1D5DB").Padding(5).Text("Month").Bold();
                           header.Cell().Background("#D1D5DB").Padding(5).Text("Total Requests").Bold();
                       });

                       // Content
                       foreach (var month in dashboard.MonthlyRequests.OrderByDescending(x => x.Key))
                       {
                           table.Cell().Padding(5).Text(month.Key);
                           table.Cell().Padding(5).Text(month.Value.ToString());
                       }
                   });

                   // Summary Section
                   col.Item().PaddingTop(30).Background("#F3F4F6").Padding(10).Column(summaryCol =>
                   {
                       summaryCol.Item().Text("Performance Summary").Bold().FontSize(16);
                       summaryCol.Item().PaddingTop(10).Text(text =>
                       {
                           var totalRequests = dashboard.ApprovedRequests + dashboard.RejectedRequests +
                                               dashboard.PendingRequests;
                           var approvalRate = totalRequests > 0
                               ? (double)dashboard.ApprovedRequests / totalRequests * 100
                               : 0;

                           var avgMonthlyEarnings = dashboard.MonthlyEarnings.Values
                               .Where(v => v.HasValue)
                               .Select(v => v.Value)
                               .DefaultIfEmpty(0)
                               .Average();

                           text.Line($"Request Approval Rate: {approvalRate:F1}%");
                           text.Line($"Average Monthly Earnings: {avgMonthlyEarnings:C}");
                           text.Line(
                               $"Net Monthly Income: {(dashboard.CurrentSalary + (dashboard.TotalBenefits ?? 0) - (dashboard.TotalDeductions ?? 0)):C}");
                       });
                   });
               });

               // Footer
               page.Footer().AlignRight().Text(text =>
               {
                   text.Span("Page ").FontSize(10);
                   text.CurrentPageNumber().FontSize(10);
                   text.Span(" of ").FontSize(10);
                   text.TotalPages().FontSize(10);
               });
           });
       }).GeneratePdf();
   }
}