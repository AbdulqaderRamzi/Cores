using System.Transactions;
using Cores.DataService.Repository.IRepository;
using Cores.Models;
using Cores.Models.Accounting;
using Cores.Models.Accounting.Enums;
using Cores.Models.ViewModels;
using Cores.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Transaction = Cores.Models.Accounting.Transaction;

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
        var transactions = await _unitOfWork.Transaction.GetAll(includeProperties: "Details.Account");
        return View(transactions);
    }

    public async Task<IActionResult> Upsert(int? id)
    {
        var transactionVm = new TransactionVm
        {
            Transaction = new(),
            Accounts = await GetActiveAccounts()
        };

        if (id is null or 0)
        {
            // New Transaction
            transactionVm.Transaction.TransactionDate = DateTime.Now;
            transactionVm.Transaction.ReferenceNo = await GenerateReferenceNumber();
            transactionVm.Transaction.Status = TransactionState.Draft.ToString();
            return View(transactionVm);
        }

        // Existing Transaction
        var transaction = await _unitOfWork.Transaction.Get(
            t => t.Id == id,
            includeProperties: "Details.Account");

        if (transaction is null)
        {
            return NotFound();
        }

        // Only draft transactions can be edited
        if (transaction.Status != TransactionState.Draft.ToString())
        {
            TempData["error"] = "Only draft transactions can be edited";
            return RedirectToAction(nameof(Index));
        }

        transactionVm.Transaction = transaction;
        return View(transactionVm);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(TransactionVm transactionVm, string serializedTransaction)
    {
    
        /*if (!ModelState.IsValid)
        {
            transactionVm.Accounts = await GetActiveAccounts();
            return View(transactionVm);
        }*/
    

        // Calculate and validate totals
        var totalDebits = transactionVm.Transaction.Details.Sum(d => d.DebitAmount);
        var totalCredits = transactionVm.Transaction.Details.Sum(d => d.CreditAmount);

        if (totalDebits != totalCredits)
        {
            ModelState.AddModelError("", "Total debits must equal total credits");
            transactionVm.Accounts = await GetActiveAccounts();
            return View(transactionVm);
        }
        
        // Validate transaction detail
        var transactionData = JsonConvert.DeserializeObject<TransactionData>(serializedTransaction);
        if (transactionData is null || transactionData.Details.Count == 0)
        {
            ModelState.AddModelError("", "Transaction must have at least one detail line");
            transactionVm.Accounts = await GetActiveAccounts();
            return View(transactionVm);
        }
        
        transactionVm.Transaction.TotalDebit = totalDebits;
        transactionVm.Transaction.TotalCredit = totalCredits;

        if (transactionVm.Transaction.Id == 0)
        {
            // New Transaction
            transactionVm.Transaction.CreatedAt = DateTime.Now;
            transactionVm.Transaction.CreatedBy = User.Identity?.Name!;
            await _unitOfWork.Transaction.Add(transactionVm.Transaction);
            await _unitOfWork.SaveAsync();

            var details = transactionData.Details;
            // Then add details with the new transaction ID
             foreach (var detail in details)
            {
                detail.TransactionId = transactionVm.Transaction.Id;
            }
            var transactionFromDb = await _unitOfWork.Transaction
                .Get(t => t.Id == transactionVm.Transaction.Id, includeProperties:"Details");
            if (transactionFromDb is not null)
            {
                transactionFromDb.Details.Clear();
                transactionFromDb.Details.AddRange(details);
            }
            TempData["success"] = "Transaction created successfully";
        }
        else
        {
            // Existing Transaction
            var existingTransaction = await _unitOfWork.Transaction.Get(
                t => t.Id == transactionVm.Transaction.Id);

            if (existingTransaction != null && existingTransaction.Status != TransactionState.Draft.ToString())
            {
                TempData["error"] = "Only draft transactions can be edited";
                return RedirectToAction(nameof(Index));
            }
            
            var details = transactionData.Details;
            // Then add details with the new transaction ID
            foreach (var detail in details)
            {
                detail.TransactionId = transactionVm.Transaction.Id;
                await _unitOfWork.TransactionDetail.Add(detail);
            }
            
            await _unitOfWork.Transaction.Update(transactionVm.Transaction);
            TempData["success"] = "Transaction updated successfully";
            await _unitOfWork.SaveAsync();
                
           
        }


        // If transaction is not in draft status, update account balances
        if (transactionVm.Transaction.Status != TransactionState.Draft.ToString())
        {
             ProcessTransaction(transactionVm.Transaction);
        }

        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> PostTransaction(int id)
    {
        var transaction = await _unitOfWork.Transaction.Get(
            t => t.Id == id,
            includeProperties: "Details.Account");

        if (transaction == null)
        {
            return NotFound();
        }

        if (transaction.Status != TransactionState.Draft.ToString())
        {
            return BadRequest("Only draft transactions can be posted");
        }

        transaction.Status = TransactionState.Posted.ToString();
        await ProcessTransaction(transaction);
        await _unitOfWork.SaveAsync();

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> VoidTransaction(int id)
    {
        var transaction = await _unitOfWork.Transaction.Get(
            t => t.Id == id,
            includeProperties: "Details.Account");

        if (transaction == null)
        {
            return NotFound();
        }

        if (transaction.Status != TransactionState.Posted.ToString())
        {
            return BadRequest("Only posted transactions can be voided");
        }

        // Create reversal transaction
        var reversalTransaction = new Transaction
        {
            ReferenceNo = await GenerateReferenceNumber(),
            TransactionDate = DateTime.Now,
            Description = $"Void of transaction {transaction.ReferenceNo}",
            Status = TransactionState.Posted.ToString(),
            CreatedAt = DateTime.Now,
            CreatedBy = User.Identity?.Name!,
            Details = transaction.Details.Select(d => new TransactionDetail
            {
                AccountId = d.AccountId,
                Description = $"Void of transaction {transaction.ReferenceNo}",
                // Swap debit and credit amounts
                DebitAmount = d.CreditAmount,
                CreditAmount = d.DebitAmount
            }).ToList()
        };

        await _unitOfWork.Transaction.Add(reversalTransaction);
        transaction.Status = TransactionState.Voided.ToString();
        await _unitOfWork.SaveAsync();

        // Process the reversal transaction
        await ProcessTransaction(reversalTransaction);
        await _unitOfWork.SaveAsync();

        return Json(new { success = true });
    }

    private async Task<string> GenerateReferenceNumber()
    {
        var today = DateTime.Now;
        var transactions = await _unitOfWork.Transaction.GetAll(
            t => t.CreatedAt.Date == today.Date);
        
        var count = transactions.Count() + 1;
        return $"TRX-{today:yyyyMMdd}-{count:D4}";
    }

    private async Task<List<SelectListItem>> GetActiveAccounts()
    {
        var accounts = await _unitOfWork.Account.GetAll(a => a.IsActive);
        return accounts.Select(a => new SelectListItem
        {
            Text = $"{a.Code} - {a.Name}",
            Value = a.Id.ToString()
        }).ToList();
    }

    private async Task ProcessTransaction(Transaction transaction)
    {
        // Create journal entry
        var journalEntry = new JournalEntry
        {
            TransactionId = transaction.Id,
            EntryNumber = await GenerateJournalEntryNumber(),
            EntryDate = transaction.TransactionDate,
            Description = transaction.Description,
            IsPosted = true,
            PostedDate = DateTime.Now,
            PostedBy = User.Identity?.Name!,
            CreatedAt = DateTime.Now,
            CreatedBy = User.Identity?.Name!,
            Details = transaction.Details.Select(d => new JournalEntryDetail
            {
                AccountId = d.AccountId,
                Description = d.Description,
                DebitAmount = d.DebitAmount,
                CreditAmount = d.CreditAmount
            }).ToList()
        };
        
        await _unitOfWork.JournalEntry.Add(journalEntry);
        await _unitOfWork.SaveAsync();
    
        foreach (var detail in transaction.Details)
        {
        
            switch (detail.Account.Type)
            {
                case "Asset" or "Expense":
                    detail.Account.Balance += detail.DebitAmount - detail.CreditAmount;
                    break;
                case "Liability" or "Equity" or "Revenue":
                    detail.Account.Balance += detail.CreditAmount - detail.DebitAmount;
                    break;
            }
            
            /*var glEntry = new GeneralLedger
            {
                AccountId = detail.AccountId,
                TransactionDate = transaction.TransactionDate,
                JournalEntryId = journalEntry.Id,
                Description = detail.Description,
                DebitAmount = detail.DebitAmount,
                CreditAmount = detail.CreditAmount,
                RunningBalance = detail.Account.Balance
            };

            await _unitOfWork.GeneralLedger.Add(glEntry);
            detail.Account.Balance = newBalance;*/
        }
    }

    private async Task<string> GenerateJournalEntryNumber()
    {
        var today = DateTime.Now;
        var entries = await _unitOfWork.JournalEntry.GetAll(
            j => j.CreatedAt.Date == today.Date);
    
        var count = entries.Count() + 1;
        return $"JE-{today:yyyyMMdd}-{count:D4}";
    }

    #region API CALLS
    [HttpGet]
    public async Task<IActionResult> GetTransactionDetails(int transactionId)
    {
        var transaction = await _unitOfWork.Transaction.Get(
            t => t.Id == transactionId,
            includeProperties: "Details.Account");

        if (transaction == null)
        {
            return NotFound();
        }

        var details = transaction.Details.Select(d => new
        {
            d.Account.Id,
            d.Account.Name,
            d.DebitAmount,
            d.CreditAmount,
            d.Description
        });

        return Json(new { data = details });
    }
    #endregion
}

