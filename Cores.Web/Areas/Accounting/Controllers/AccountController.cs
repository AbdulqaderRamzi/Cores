using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
using Cores.Models.ViewModels;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Web.Areas.Accounting.Controllers;

[Area("Accounting")]
[Authorize(Roles = SD.ACCOUNTING_ROLE + "," + SD.ADMIN_ROLE)]
public class AccountController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public AccountController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<IActionResult> Index()
    {
        var accounts = await _unitOfWork.Account.GetAll(includeProperties:"AccountType");
        return View(accounts);
    }
    
    public async Task<IActionResult> Upsert(int id)
    {
        var accountVm = new AccountVm()
        {
            Account = new()
        };
        await FillSelectionData(accountVm);

        if (id is 0)
            return View(accountVm);
        
        var account = await _unitOfWork.Account.Get(t => t.Id == id, includeProperties: "AccountType");
        if (account is null)
            return NotFound();
        accountVm.Account = account;
        return View(accountVm);
    }
    
    [HttpPost]
    public async Task<IActionResult> Upsert(AccountVm accountVm)
    {
        if (!ModelState.IsValid)
        {
            await FillSelectionData(accountVm);
            return View(accountVm);
        }

        if (accountVm.Account.Id is 0)
        {
            await _unitOfWork.Account.Add(accountVm.Account);
            TempData["success"] = "Account created successfully";
        }
        else
        {
            await _unitOfWork.Account.Update(accountVm.Account);
            TempData["success"] = "Account updated successfully";
        }
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
            return NotFound();

        var account = await _unitOfWork.Account.Get(t => t.Id == id);
        if (account is null)
            return NotFound();
        _unitOfWork.Account.Remove(account);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Account deleted successfully";
        return RedirectToAction(nameof(Index));
    }
    
    private async Task FillSelectionData(AccountVm accountVm)
    {
        var accountTypes = await _unitOfWork.AccountType.GetAll();
        accountVm.AccountTypes = accountTypes.Select(at => new SelectListItem
        {
            Text = at.Type,
            Value = at.Id.ToString()
        }).ToList();
    }
}