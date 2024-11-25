using Cores.Models;
using Cores.Utilities;
using Hangfire;
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
    public IActionResult Index(EmailPayload emailPayload)
    {
        BackgroundJob.Enqueue(() => _mailer.SendEmailAsync(emailPayload));
        return View();
    }
}