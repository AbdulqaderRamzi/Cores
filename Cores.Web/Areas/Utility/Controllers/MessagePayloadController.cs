using System.Security.Claims;
using Cores.DataService.Repository.IRepository;
using Cores.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.Utility.Controllers;

[Area("Utility")]
[Authorize]
public class MessagePayloadController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public MessagePayloadController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    public IActionResult Send(string? employeeEmail = null)
    {
        var message = new MessagePayload();
        if (employeeEmail is not null)
        {
            message.RecipientEmail = employeeEmail;
        }
        return View(message);
    }

    [HttpPost]
    public async Task<IActionResult> Send(MessagePayload messagePayload)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Send));
        var claimsIdentity = (ClaimsIdentity)User.Identity!;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        messagePayload.SenderId = userId;
        messagePayload.Seen = false;
        messagePayload.RecipientEmail = messagePayload.RecipientEmail.ToLower().Trim();
        await _unitOfWork.MessagePayload.Add(messagePayload);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "The message has sent successfully";
        return RedirectToAction(nameof(Send));
    }
}