using Cores.DataService.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.ViewComponents;

public class MessageViewComponent : ViewComponent
{
    private readonly IUnitOfWork _unitOfWork;

    public MessageViewComponent(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IViewComponentResult> InvokeAsync(string? receiverEmail)
    {
        var messages = await _unitOfWork.MessagePayload
            .GetAll(m => m.RecipientEmail == receiverEmail && !m.Seen, "ApplicationUser");
        return View(messages.OrderByDescending(m => m.SentAt).ToList());
    }
}