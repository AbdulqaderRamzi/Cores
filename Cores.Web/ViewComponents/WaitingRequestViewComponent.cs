using Cores.DataService.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.ViewComponents;

public class WaitingRequestViewComponent : ViewComponent
{
    private readonly IUnitOfWork _unitOfWork;

    public WaitingRequestViewComponent(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IViewComponentResult> InvokeAsync(string userId)
    { 
        var employee = await _unitOfWork.ApplicationUser.Get(e => e.Id == userId);
        return View(employee);
    }
}