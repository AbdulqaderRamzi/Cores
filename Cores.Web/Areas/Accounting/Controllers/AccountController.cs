using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
using Cores.Models.Accounting.Enums;
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
        var accounts = await _unitOfWork.Account.GetAll();
        return View(accounts);
    }

    public async Task<IActionResult> Upsert(int? id)
    {
        var accountVm = new AccountVm
        {
            Account = new(),
        };
        FillSelectionData(accountVm);

        if (id is null or 0)
        {
            accountVm.Account.CreatedAt = DateTime.Now;
            accountVm.Account.CreatedBy = User.Identity?.Name!;
            accountVm.Account.Code = await GenerateAccountCode();
            return View(accountVm);
        }

        var account = await _unitOfWork.Account.Get(a => a.Id == id);   
        if (account is null)
        {
            return NotFound();
        }

        accountVm.Account = account;
        return View(accountVm);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(AccountVm accountVm)
    {
        if (!ModelState.IsValid)
        {
            FillSelectionData(accountVm);
            return View(accountVm);
        }

        if (accountVm.Account.Id == 0)
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
        {
            return NotFound();
        }

        var payroll = await _unitOfWork.Account.Get(p => p.Id == id);
        if (payroll is null)
        {
            return NotFound();
        }

        _unitOfWork.Account.Remove(payroll);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Accoutn deleted successfully";
        return RedirectToAction(nameof(Index));
    }

    private async Task<string> GenerateAccountCode()
    {
        var accounts = await _unitOfWork.Account.GetAll();
        var lastAccount = accounts.MaxBy(a => a.Code);
        if (lastAccount == null) return "1000";

        var lastCode = int.Parse(lastAccount.Code);
        return (lastCode + 100).ToString();
    }
    private static void FillSelectionData(AccountVm accountVm)
    {
        accountVm.AccountTypes = Enum.GetValues(typeof(AccountType))
            .Cast<AccountType>()
            .Select(t => new SelectListItem
            {
                Text = t.ToString(),
                Value = t.ToString()
            }).ToList();
        accountVm.AccountCategories = Enum.GetValues(typeof(AccountCategory))
            .Cast<AccountCategory>()
            .Select(c => new SelectListItem
            {
                Text = c.ToString(),
                Value = c.ToString()
            }).ToList();
    }
    

    #region API CALLS
    [HttpGet]
    public async Task<IActionResult> GetAccountTransactions(int accountId)
    {
        // Fetch transactions for the specific account
        var transactions = await _unitOfWork.TransactionDetail.GetAll(
            td => td.AccountId == accountId,
            includeProperties: "Transaction");
    
        // Map transactions to a view-friendly format
        var response = transactions.Select(t => new 
        {
            date = t.Transaction.TransactionDate.ToString("yyyy-MM-dd"),
            referenceNo = t.Transaction.ReferenceNo,
            description = t.Description,
            debit = t.DebitAmount,
            credit = t.CreditAmount
        }).ToList();
    
        // Return JSON with a data property
        return Json(new { 
            data = response 
        });
    }
    #endregion
}
    
   
