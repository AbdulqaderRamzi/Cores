using System.Security.Claims;
using Cores.DataService.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.Employee.Controllers;

[Area("Employee")]
[Authorize]
public class ActivityLogController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public ActivityLogController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity!;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var activities = await _unitOfWork.ActivityLog
            .GetAll(u => u.ApplicationUserId == userId);
        return View(activities);
    }
}