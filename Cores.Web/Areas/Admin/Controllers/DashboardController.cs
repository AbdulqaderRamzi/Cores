using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.ADMIN_ROLE)]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}