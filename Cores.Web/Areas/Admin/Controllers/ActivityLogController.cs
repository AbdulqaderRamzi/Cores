using Cores.DataService.Repository.IRepository;
using Cores.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.ADMIN_ROLE)]
public class ActivityLogController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public ActivityLogController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    public async Task<IActionResult> Index()
    {
        var activities =
            await _unitOfWork.ActivityLog.GetAll(includeProperties: "ApplicationUser");
        return View(activities);
    }
}