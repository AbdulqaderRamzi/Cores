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
    private readonly Dictionary<string, (string area, string controller)> _roleRedirects = new()
    {
        { SD.ADMIN_ROLE, ("Hr", "Dashboard") },
        { SD.HR_ROLE, ("Hr", "Dashboard") },
        { SD.ACCOUNTING_ROLE, ("Accounting", "Dashboard") },
        { SD.CRM_ROLE, ("CRM", "Dashboard") }
    };

    public IActionResult Index()
    {
        var userRole = User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        return _roleRedirects.TryGetValue(userRole ?? "", out var redirect) 
            ? RedirectToAction("Index", redirect.controller, new { area = redirect.area }) 
            : RedirectToAction("Index", "Dashboard", new { area = "Employee" });
    }

    [AllowAnonymous]
    public IActionResult Welcome()
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return RedirectToAction(nameof(Index));
        }
        
        return RedirectToPage("/Account/Login", new { area = "Identity" });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel 
        { 
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
        });
    }
}