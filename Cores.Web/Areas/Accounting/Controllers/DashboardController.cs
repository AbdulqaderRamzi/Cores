using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.Accounting.Controllers;

[Area("Accounting")]
[Authorize(Roles = SD.ACCOUNTING_ROLE)]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}