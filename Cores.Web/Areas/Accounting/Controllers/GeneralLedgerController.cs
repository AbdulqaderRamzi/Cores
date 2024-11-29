using Cores.DataService.Data;
using Cores.DataService.Repository.IRepository;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.Accounting.Controllers;

[Area("Accounting")]
[Authorize(Roles = SD.ACCOUNTING_ROLE + "," + SD.ADMIN_ROLE)]
public class GeneralLedgerController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public GeneralLedgerController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var ledgers = await _unitOfWork.GeneralLedger
            .GetAll(includeProperties:"Account,JournalEntry");
        return View(ledgers);
    }
}