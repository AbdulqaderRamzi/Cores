using Cores.Models;
using Cores.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.Utility.Controllers;

[Area("Utility")]
[Authorize]
public class MailerController : Controller
{
    private readonly IMailer _mailer;

    public MailerController(IMailer mailer)
    {
        _mailer = mailer;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(EmailPayload emailPayload)
    {
        await _mailer.SendEmailAsync(emailPayload);
        return View();
    }
}