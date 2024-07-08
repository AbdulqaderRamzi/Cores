using System.Security.Claims;
using Cores.DataService.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.Utility.Controllers;

[Area("Utility")]
[Authorize]
public class NotificationController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public NotificationController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var notifications = await _unitOfWork.Notification
            .GetAll(n => n.Todo.ApplicationUserId == userId && !n.IsRead,"Todo");
        return View(notifications.OrderByDescending(n => n.DateTime).ToList());
    }


    #region API CALL

    [HttpPost]
    public async Task<IActionResult> MarkAsRead(int notificationId)
    {
        var notification = await _unitOfWork.Notification.Get(n => n.Id == notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            await _unitOfWork.SaveAsync();
            return Ok();
        }
        return NotFound();
    }

    #endregion

   
}