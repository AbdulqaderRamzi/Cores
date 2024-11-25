using System.Diagnostics;
using System.Security.Claims;
using Cores.Models;
using Cores.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.Employee.Controllers;

[Area("Employee")]
[Authorize]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return User.IsInRole(SD.ADMIN_ROLE) ? RedirectToAction(nameof(Index), "Dashboard", new { area = "Admin"}) :
            User.IsInRole(SD.ACCOUNTING_ROLE) ? RedirectToAction(nameof(Index), "Dashboard", new { area = "Accounting"}) :
            User.IsInRole(SD.CRM_ROLE) ? RedirectToAction(nameof(Index), "Dashboard", new { area = "CRM"}) :
            User.IsInRole(SD.HR_ROLE) ? RedirectToAction(nameof(Index), "Dashboard", new { area = "HR"}) :
            RedirectToAction(nameof(Index), "Dashboard", new { area = "Employee"});
    }

    [AllowAnonymous]
    public IActionResult Welcome()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity!;
        var employeeId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (employeeId is not null)
            return RedirectToAction(nameof(Index));
        return RedirectToPage("/Account/Login", new { area = "Identity" });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}