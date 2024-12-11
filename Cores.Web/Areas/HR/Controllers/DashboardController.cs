using System.Security.Claims;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Cores.Utilities;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;

namespace Cores.Web.Areas.HR.Controllers;

[Area("HR")]
[Authorize(Roles = SD.HR_ROLE + "," + SD.ADMIN_ROLE)]
public class DashboardController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public DashboardController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        
        var employees = await _unitOfWork.ApplicationUser
            .GetAll(e => e.Email != "admin@cores.com");
        var employeesList = employees.ToList();
        var employeeCount = employeesList.Count;
        var hrCount = employeesList.Count(e => e.Role == SD.HR_ROLE);
        var crmCount = employeesList.Count(e => e.Role == SD.CRM_ROLE);
        var accountingCount = employeesList.Count(e => e.Role == SD.ACCOUNTING_ROLE);

        var todos = await _unitOfWork.Todo.GetAll(t => t.Role == SD.HR_ROLE);
        var todosList = todos.ToList();
        var inProgressTaskCount = todosList.Count(t => t.Status == "In Progress");
        var completedTaskCount = todosList.Count(t => t.Status == "Complete");
        var failedTaskCount = todosList.Count(t => t.Status == "Failed");
                
        var lateEmployees = await _unitOfWork.Attendance.GetMonthlyLateEmployees();
        var jobApplications = await _unitOfWork.JobApplication.GetMonthlyJobApplications();

        var hrDashboard = new HrDashboard
        {
            EmployeesCount = employeeCount,
            HrCount = hrCount,
            CrmCount = crmCount,
            AccountingCount = accountingCount,
            InProgressTodoCount = inProgressTaskCount,    
            CompletedTodoCount = completedTaskCount,
            FailedTodoCount = failedTaskCount,
            LateEmployees = lateEmployees,
            JobApplications = jobApplications
        };
        
        return View(hrDashboard);
    }
    
    [HttpGet]
    public async Task<IActionResult> GenerateReport()
    {
        var employees = await _unitOfWork.ApplicationUser
            .GetAll(e => e.Email != "admin@cores.com");
        var employeesList = employees.ToList();
        var employeeCount = employeesList.Count;
        var hrCount = employeesList.Count(e => e.Role == SD.HR_ROLE);
        var crmCount = employeesList.Count(e => e.Role == SD.CRM_ROLE);
        var accountingCount = employeesList.Count(e => e.Role == SD.ACCOUNTING_ROLE);

        var todos = await _unitOfWork.Todo.GetAll(t => t.Role == SD.HR_ROLE);
        var todosList = todos.ToList();
        var inProgressTaskCount = todosList.Count(t => t.Status == "In Progress");
        var completedTaskCount = todosList.Count(t => t.Status == "Complete");
        var failedTaskCount = todosList.Count(t => t.Status == "Failed");
            
        var lateEmployees = await _unitOfWork.Attendance.GetMonthlyLateEmployees();
        var jobApplications = await _unitOfWork.JobApplication.GetMonthlyJobApplications();

        var hrDashboard = new HrDashboard
        {
            EmployeesCount = employeeCount,
            HrCount = hrCount,
            CrmCount = crmCount,
            AccountingCount = accountingCount,
            InProgressTodoCount = inProgressTaskCount,    
            CompletedTodoCount = completedTaskCount,
            FailedTodoCount = failedTaskCount,
            LateEmployees = lateEmployees,
            JobApplications = jobApplications
        };

        var pdfData = GenerateDashboardReport(hrDashboard);
        return File(pdfData, "application/pdf", $"HR_Dashboard_Report_{DateTime.Now:yyyyMMdd}.pdf");
    }
    
    public static byte[] GenerateDashboardReport(HrDashboard dashboard)
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
                        col.Item().Text("HR Dashboard Report").Bold().FontSize(24);
                        col.Item().Text($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(12);
                    });
                });

                page.Content().Column(col =>
                {
                    // Workforce Overview Section
                    col.Item().PaddingTop(20).Text("Workforce Overview").Bold().FontSize(20);
                    col.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        // Department Distribution
                        table.Cell().Background("#EEF2FF").Padding(10).Column(cell =>
                        {
                            cell.Item().Text("Total Employees").Bold();
                            cell.Item().PaddingTop(5).Text(dashboard.EmployeesCount.ToString());
                        });

                        table.Cell().Background("#FEF2F2").Padding(10).Column(cell =>
                        {
                            cell.Item().Text("HR Department").Bold();
                            cell.Item().PaddingTop(5).Text(dashboard.HrCount.ToString());
                            cell.Item().Text($"{CalculatePercentage(dashboard.HrCount, dashboard.EmployeesCount):P1}");
                        });

                        table.Cell().Background("#ECFDF5").Padding(10).Column(cell =>
                        {
                            cell.Item().Text("CRM Department").Bold();
                            cell.Item().PaddingTop(5).Text(dashboard.CrmCount.ToString());
                            cell.Item().Text($"{CalculatePercentage(dashboard.CrmCount, dashboard.EmployeesCount):P1}");
                        });

                        table.Cell().Background("#FEF3C7").Padding(10).Column(cell =>
                        {
                            cell.Item().Text("Accounting Dept.").Bold();
                            cell.Item().PaddingTop(5).Text(dashboard.AccountingCount.ToString());
                            cell.Item().Text($"{CalculatePercentage(dashboard.AccountingCount, dashboard.EmployeesCount):P1}");
                        });
                    });

                    // Task Management Section
                    col.Item().PaddingTop(30).Text("Task Management Overview").Bold().FontSize(20);
                    col.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        var totalTasks = dashboard.CompletedTodoCount + dashboard.InProgressTodoCount + dashboard.FailedTodoCount;

                        table.Cell().Background("#DCF7E3").Padding(10).Column(cell =>
                        {
                            cell.Item().Text("Completed Tasks").Bold();
                            cell.Item().PaddingTop(5).Text(dashboard.CompletedTodoCount.ToString());
                            cell.Item().Text($"{CalculatePercentage(dashboard.CompletedTodoCount, totalTasks):P1}");
                        });

                        table.Cell().Background("#FEF3C7").Padding(10).Column(cell =>
                        {
                            cell.Item().Text("In Progress Tasks").Bold();
                            cell.Item().PaddingTop(5).Text(dashboard.InProgressTodoCount.ToString());
                            cell.Item().Text($"{CalculatePercentage(dashboard.InProgressTodoCount, totalTasks):P1}");
                        });

                        table.Cell().Background("#FEE2E2").Padding(10).Column(cell =>
                        {
                            cell.Item().Text("Failed Tasks").Bold();
                            cell.Item().PaddingTop(5).Text(dashboard.FailedTodoCount.ToString());
                            cell.Item().Text($"{CalculatePercentage(dashboard.FailedTodoCount, totalTasks):P1}");
                        });
                    });

                    // Attendance Trends Section
                    col.Item().PaddingTop(30).Text("Monthly Attendance Report").Bold().FontSize(20);
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
                            header.Cell().Background("#D1D5DB").Padding(5).Text("Late Arrivals").Bold();
                        });

                        // Content
                        foreach (var month in dashboard.LateEmployees.OrderByDescending(x => x.Key))
                        {
                            table.Cell().Padding(5).Text(month.Key);
                            table.Cell().Padding(5).Text(month.Value.ToString());
                        }
                    });

                    // Job Applications Section
                    col.Item().PaddingTop(30).Text("Recruitment Overview").Bold().FontSize(20);
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
                            header.Cell().Background("#D1D5DB").Padding(5).Text("Applications").Bold();
                        });

                        // Content
                        foreach (var month in dashboard.JobApplications.OrderByDescending(x => x.Key))
                        {
                            table.Cell().Padding(5).Text(month.Key);
                            table.Cell().Padding(5).Text(month.Value.ToString());
                        }
                    });

                    // Summary Section
                    col.Item().PaddingTop(30).Background("#F3F4F6").Padding(10).Column(summaryCol =>
                    {
                        summaryCol.Item().Text("Performance Metrics").Bold().FontSize(16);
                        summaryCol.Item().PaddingTop(10).Text(text =>
                        {
                            var taskCompletionRate = CalculatePercentage(dashboard.CompletedTodoCount, 
                                dashboard.CompletedTodoCount + dashboard.InProgressTodoCount + dashboard.FailedTodoCount);
                            
                            var totalLateEmployees = dashboard.LateEmployees.Sum(x => x.Value);
                            var avgMonthlyLateEmployees = dashboard.LateEmployees.Any() 
                                ? (double)totalLateEmployees / dashboard.LateEmployees.Count 
                                : 0;

                            text.Line($"Task Completion Rate: {taskCompletionRate:P1}");
                            text.Line($"Average Monthly Late Arrivals: {avgMonthlyLateEmployees:F1}");
                            text.Line($"Total Job Applications: {dashboard.JobApplications.Sum(x => x.Value)}");
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

    private static decimal CalculatePercentage(int part, int total)
    {
        return total == 0 ? 0 : (decimal)part / total;
    }
}