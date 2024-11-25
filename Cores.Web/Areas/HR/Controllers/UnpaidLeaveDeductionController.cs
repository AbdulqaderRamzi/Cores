using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.HR.Controllers;

[Area("HR")]
[Authorize(Roles = SD.HR_ROLE + "," + SD.ADMIN_ROLE)]
public class UnpaidLeaveDeductionController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    

    public UnpaidLeaveDeductionController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var unpaidLeaveDeductions = await _unitOfWork.UnpaidLeaveDeduction.GetAll();
        return View(unpaidLeaveDeductions);
    }
}