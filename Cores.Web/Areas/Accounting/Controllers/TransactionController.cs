using System.Security.Claims;
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
public class TransactionController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<IActionResult> Index()
    {
        var transactions = await _unitOfWork.Transaction.GetAll(includeProperties: "DebitAccount,CreditAccount,ApplicationUser");
        return View(transactions);
    }

    public async Task<IActionResult> Upsert(int id)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity!;
        var employeeId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (employeeId is null)
            return NotFound();
        var employee = await _unitOfWork.ApplicationUser.Get(a => a.Id == employeeId);
        if (employee is null)
            return NotFound();
            
        var transactionVm = new TransactionVm
        {
            Transaction = new Transaction
            {
                ApplicationUserId = employeeId
            },
            EmployeeName = $"{employee.FirstName} {employee.LastName}"
        };
        await FillSelectionData(transactionVm);
       
        if (id is 0)
            return View(transactionVm);
        
        var transaction = await _unitOfWork.Transaction.Get(t => t.Id == id, includeProperties: "DebitAccount,CreditAccount,ApplicationUser");
        if (transaction is null)
            return NotFound();
        transactionVm.Transaction = transaction;
        return View(transactionVm);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(TransactionVm transactionVm)
    {
        if (!ModelState.IsValid)
        {
            await FillSelectionData(transactionVm);
            return View(transactionVm);
        }

        if (transactionVm.Transaction.Id is 0)
        {
            await _unitOfWork.Transaction.Add(transactionVm.Transaction);
            TempData["success"] = "Transaction created successfully";
        }
        else
        {
            await _unitOfWork.Transaction.Update(transactionVm.Transaction);
            TempData["success"] = "Transaction updated successfully";
        }
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
            return NotFound();

        var transaction = await _unitOfWork.Transaction.Get(t => t.Id == id);
        if (transaction is null)
            return NotFound();

        _unitOfWork.Transaction.Remove(transaction);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Transaction deleted successfully";
        return RedirectToAction(nameof(Index));
    }

    private async Task FillSelectionData(TransactionVm transactionVm)
    {
        var accounts = await _unitOfWork.Account.GetAll();
        transactionVm.Accounts = accounts.Select(a => new SelectListItem
        {
            Text = a.Name,
            Value = a.Id.ToString()
        });
    }
}