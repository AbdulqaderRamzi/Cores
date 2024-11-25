using Cores.DataService.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.ViewComponents;

public class EmployeeViewComponent : ViewComponent
{
    private readonly IUnitOfWork _unitOfWork;

    public EmployeeViewComponent(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IViewComponentResult> InvokeAsync(string? id)
    {
        var employee = await _unitOfWork.ApplicationUser.Get(u => u.Id == id);

        return View("Default", employee?.FirstName);
    }
}