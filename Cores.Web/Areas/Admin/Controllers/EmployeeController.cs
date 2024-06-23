using Cores.DataService.Repository.IRepository;
using Cores.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.ADMIN_ROLE)]
public class EmployeeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public EmployeeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var applicationUsers = await _unitOfWork.ApplicationUser.GetAll();
        return View(applicationUsers);
    }

    public async Task<IActionResult> Details(string? id)
    {
        if (id is null)
            return RedirectToAction(nameof(Index));
        var employee = await _unitOfWork.ApplicationUser.Get(a => a.Id == id, "Languages");
        if (employee is null)
            return RedirectToAction(nameof(Index));
        return View(employee);
    }

    #region API CALL

    [HttpPost]
    public async Task<IActionResult> LockUnlock(string? id)
    {
        if (string.IsNullOrEmpty(id))
            return Json(new { success = false, message = "Invalid user ID" });

        var user = await _unitOfWork.ApplicationUser.Get(a => a.Id == id);

        if (user is null)
            return Json(new { success = false, message = "User not found" });

        var isLocked = user.LockoutEnd is not null && user.LockoutEnd > DateTime.Now;
        user.LockoutEnd = isLocked ? DateTime.Now : DateTime.Now.AddYears(1000);

        await _unitOfWork.SaveAsync();

        return Json(new { success = true, message = "Operation Success", isLocked = !isLocked });
    }

    #endregion
}