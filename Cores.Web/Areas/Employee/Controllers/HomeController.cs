using System.Diagnostics;
using System.Security.Claims;
using Cores.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.Employee.Controllers;

[Area("Employee")]
[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
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