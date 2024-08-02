using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
using Cores.Models.CRM;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.Accounting.Controllers;

[Area("Accounting")]
[Authorize(Roles = SD.ACCOUNTING_ROLE + "," + SD.ADMIN_ROLE)]
public class AccountTypeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public AccountTypeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var accountTypes = await _unitOfWork.AccountType.GetAll();
        return View(accountTypes);
    }

    public async Task<IActionResult> Upsert(int id)
    {
        if (id is 0)
        {
            return View(new AccountType());
        }

        var accountType = await _unitOfWork.AccountType.Get(at => at.Id == id);
        if (accountType is null)
            return NotFound();
        return View(accountType);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(AccountType accountType)
    {
        if (!ModelState.IsValid)
            return View(accountType);
        if (accountType.Id is 0)
        {
            await _unitOfWork.AccountType.Add(accountType);
            TempData["success"] = "Account Type added successfully";
        }
        else
        {
            await _unitOfWork.AccountType.Update(accountType);
            TempData["success"] = "Account Type updated successfully";
        }

        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        if (id is 0)
            return BadRequest();
        var accountType = await _unitOfWork.AccountType.Get(at => at.Id == id);
        if (accountType is null)
            return NotFound();
        _unitOfWork.AccountType .Remove(accountType);
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
}