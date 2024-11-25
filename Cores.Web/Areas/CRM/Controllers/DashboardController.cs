using Cores.DataService.Repository.IRepository;
using Cores.Models.CRM;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.CRM.Controllers;

[Area("CRM")]
[Authorize(Roles = SD.CRM_ROLE)]
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
}