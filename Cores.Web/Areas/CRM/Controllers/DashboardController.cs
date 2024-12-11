using Cores.DataService.Repository.IRepository;
using Cores.Models.CRM;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;

namespace Cores.Web.Areas.CRM.Controllers;

[Area("CRM")]
[Authorize(Roles = SD.CRM_ROLE + "," + SD.ADMIN_ROLE)]
public class DashboardController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public DashboardController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var contacts = await _unitOfWork.Contact.GetStatusCounts();
        var pendingEvents = await _unitOfWork.Event.PendingEventsCount();
        var problems = await _unitOfWork.Problem.GetStatusCounts();
        var monthlyPurchase  = await _unitOfWork.Purchase.GetMonthlyPurchase();
        var monthlyEarnings = await _unitOfWork.Purchase.GetMonthlyEarnings();
        var monthlyContacts = await _unitOfWork.Contact.GetMonthlyContacts();
        CrmDashboard dashboard = new()
        {
            CustomersCount = contacts.CustomerCount,
            LeadsCount = contacts.LeadCount,
            PendingEvents = pendingEvents,
            OpenProblems = problems.OpenProblems,
            PendingProblems = problems.PendingProblems,
            ClosedProblems = problems.ClosedProblems,
            MonthlyPurchase = monthlyPurchase,
            MonthlyEarnings = monthlyEarnings, 
            MonthlyContacts = monthlyContacts
        };
        return View(dashboard);
    }
    
    [HttpGet]
    public async Task<IActionResult> GenerateReport()
    {
        var contacts = await _unitOfWork.Contact.GetStatusCounts();
        var pendingEvents = await _unitOfWork.Event.PendingEventsCount();
        var problems = await _unitOfWork.Problem.GetStatusCounts();
        var monthlyPurchase = await _unitOfWork.Purchase.GetMonthlyPurchase();
        var monthlyEarnings = await _unitOfWork.Purchase.GetMonthlyEarnings();
        var monthlyContacts = await _unitOfWork.Contact.GetMonthlyContacts();
    
        CrmDashboard dashboard = new()
        {
            CustomersCount = contacts.CustomerCount,
            LeadsCount = contacts.LeadCount,
            PendingEvents = pendingEvents,
            OpenProblems = problems.OpenProblems,
            PendingProblems = problems.PendingProblems,
            ClosedProblems = problems.ClosedProblems,
            MonthlyPurchase = monthlyPurchase,
            MonthlyEarnings = monthlyEarnings,
            MonthlyContacts = monthlyContacts
        };

        var pdfData = GenerateDashboardReport(dashboard);
        return File(pdfData, "application/pdf", $"CRM_Dashboard_Report_{DateTime.Now:yyyyMMdd}.pdf");
    }

    public static byte[] GenerateDashboardReport(CrmDashboard dashboard)
    {
        var data = Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Margin(20);
                
                // Header
                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("CRM Dashboard Report").Bold().FontSize(24);
                        col.Item().Text($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(12);
                    });
                });

                page.Content().Column(col =>
                {
                    // Key Metrics Section
                    col.Item().PaddingTop(20).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        // Contacts Stats
                        table.Cell().Background("#EEF2FF").Padding(10).Column(cell =>
                        {
                            cell.Item().Text("Contact Statistics").Bold();
                            cell.Item().PaddingTop(5).Text($"Customers: {dashboard.CustomersCount}");
                            cell.Item().Text($"Leads: {dashboard.LeadsCount}");
                            cell.Item().Text($"Total: {dashboard.CustomersCount + dashboard.LeadsCount}");
                        });

                        // Problems Stats
                        table.Cell().Background("#FEF2F2").Padding(10).Column(cell =>
                        {
                            cell.Item().Text("Problem Tickets").Bold();
                            cell.Item().PaddingTop(5).Text($"Open: {dashboard.OpenProblems}");
                            cell.Item().Text($"Pending: {dashboard.PendingProblems}");
                            cell.Item().Text($"Closed: {dashboard.ClosedProblems}");
                        });

                        // Events Stats
                        table.Cell().Background("#ECFDF5").Padding(10).Column(cell =>
                        {
                            cell.Item().Text("Events").Bold();
                            cell.Item().PaddingTop(5).Text($"Pending Events: {dashboard.PendingEvents}");
                            cell.Item().Text($"Monthly Purchases: {dashboard.MonthlyPurchase}");
                        });
                    });

                    // Monthly Earnings Section
                    col.Item().PaddingTop(30).Text("Monthly Earnings").Bold().FontSize(20);
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
                            header.Cell().Background("#D1D5DB").Padding(5).Text("Earnings").Bold();
                        });

                        // Content
                        foreach (var earning in dashboard.MonthlyEarnings.OrderByDescending(x => x.Key))
                        {
                            table.Cell().Padding(5).Text(earning.Key);
                            table.Cell().Padding(5).Text($"{earning.Value:C}");
                        }
                    });

                    // Monthly Contacts Section
                    col.Item().PaddingTop(30).Text("Monthly Contact Growth").Bold().FontSize(20);
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
                            header.Cell().Background("#D1D5DB").Padding(5).Text("New Contacts").Bold();
                        });

                        // Content
                        foreach (var contact in dashboard.MonthlyContacts.OrderByDescending(x => x.Key))
                        {
                            table.Cell().Padding(5).Text(contact.Key);
                            table.Cell().Padding(5).Text(contact.Value.ToString());
                        }
                    });

                    // Summary Section
                    col.Item().PaddingTop(30).Background("#F3F4F6").Padding(10).Column(summaryCol =>
                    {
                        summaryCol.Item().Text("Performance Summary").Bold().FontSize(16);
                        summaryCol.Item().PaddingTop(10).Text(text =>
                        {
                            // Calculate total earnings
                            var totalEarnings = dashboard.MonthlyEarnings.Sum(x => x.Value);
                            var avgMonthlyEarnings = dashboard.MonthlyEarnings.Any() 
                                ? totalEarnings / dashboard.MonthlyEarnings.Count 
                                : 0;

                            text.Line($"Total Earnings: {totalEarnings:C}");
                            text.Line($"Average Monthly Earnings: {avgMonthlyEarnings:C}");
                            text.Line($"Problem Resolution Rate: {CalculateResolutionRate(dashboard):P1}");
                            text.Line($"Customer to Lead Ratio: {CalculateCustomerLeadRatio(dashboard):P1}");
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
        });

        return data.GeneratePdf();
    }

    private static decimal CalculateResolutionRate(CrmDashboard dashboard)
    {
        var totalProblems = dashboard.OpenProblems + dashboard.PendingProblems + dashboard.ClosedProblems;
        return totalProblems == 0 ? 0 : (decimal)dashboard.ClosedProblems / totalProblems;
    }

    private static decimal CalculateCustomerLeadRatio(CrmDashboard dashboard)
    {
        var totalContacts = dashboard.CustomersCount + dashboard.LeadsCount;
        return totalContacts == 0 ? 0 : (decimal)dashboard.CustomersCount / totalContacts;
    }
}