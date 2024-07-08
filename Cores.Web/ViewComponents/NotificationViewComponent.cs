using Cores.DataService.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.ViewComponents;

public class NotificationViewComponent : ViewComponent
{
    private readonly IUnitOfWork _unitOfWork;

    public NotificationViewComponent(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<IViewComponentResult> InvokeAsync(string userId)
    {
        var notifications = await _unitOfWork.Notification
            .GetAll(n => n.Todo.ApplicationUserId == userId && !n.IsRead, "Todo");
        return View(notifications.OrderByDescending(n => n.DateTime));
    }
}