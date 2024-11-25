using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.Employee.Controllers;

[Area("Employee")]
[Authorize]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}