using System.Security.Claims;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Cores.Utilities;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;

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
}