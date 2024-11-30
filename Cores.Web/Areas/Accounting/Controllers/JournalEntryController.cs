using System.Security.Claims;
using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
using Cores.Models.Accounting.Enums;
using Cores.Models.ViewModels;
using Cores.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Web.Areas.Accounting.Controllers;

[Area("Accounting")]
[Authorize(Roles = SD.ACCOUNTING_ROLE + "," + SD.ADMIN_ROLE)]
public class JournalEntryController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public JournalEntryController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var journalEntries = await _unitOfWork.JournalEntry.GetAll();
        return View(journalEntries);
    }

    /*public async Task<IActionResult> Upsert(int? id, int? journalId)
    {
        JournalEntryVm entryVm = new()
        {
            JournalEntry = new(),
            Accounts = await GetActiveAccounts()
        };

        // Edit mode
        if (id is > 0)
        {
            var entry = await _unitOfWork.JournalEntry.Get(
                e => e.Id == id,
                includeProperties: "Details"
            );
            
            if (entry is null) return NotFound();
            
            // Prevent editing of posted entries
            if (entry.IsPosted)
            {
                TempData["error"] = "Posted entries cannot be modified";
                return RedirectToAction("Index", "Journal");
            }

            entryVm.JournalEntry = entry;
        }
        // Create mode
        else if (journalId is > 0)
        {
            var journal = await _unitOfWork.Journal.Get(j => j.Id == journalId);
            if (journal is null) return NotFound();

            entryVm.JournalEntry = new()
            {
                JournalId = journalId.Value,
                EntryDate = DateTime.Now,
                EntryNumber = await GenerateEntryNumber(journalId.Value)
            };
        }
        else
        {
            return NotFound();
        }

        return View(entryVm);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(JournalEntryVm entryVm)
    {
        if (!ModelState.IsValid)
        {
            entryVm.Accounts = await GetActiveAccounts();
            return View(entryVm);
        }

        // Validate debits equal credits
        var totalDebits = entryVm.JournalEntry.Details.Sum(d => d.DebitAmount);
        var totalCredits = entryVm.JournalEntry.Details.Sum(d => d.CreditAmount);

        if (totalDebits != totalCredits)
        {
            ModelState.AddModelError("", "Total debits must equal total credits");
            entryVm.Accounts = await GetActiveAccounts();
            return View(entryVm);
        }

        // Create mode
        if (entryVm.JournalEntry.Id is 0)
        {
            entryVm.JournalEntry.CreatedAt = DateTime.Now;
            entryVm.JournalEntry.CreatedBy = User.Identity?.Name!;
            await _unitOfWork.JournalEntry.Add(entryVm.JournalEntry);
        }
        // Edit mode
        else
        {
            var existingEntry = await _unitOfWork.JournalEntry.Get(
                e => e.Id == entryVm.JournalEntry.Id,
                includeProperties: "Details"
            );
            
            if (existingEntry == null) return NotFound();
            
            if (existingEntry.IsPosted)
            {
                TempData["error"] = "Posted entries cannot be modified";
                return RedirectToAction("Index", "Journal");
            }

            // Remove existing details
            foreach (var detail in existingEntry.Details.ToList())
            {
                /*
                await _unitOfWork.JournalEntryDetail.Remove(detail);
            #1#
            }

            // Update entry properties
            existingEntry.EntryDate = entryVm.JournalEntry.EntryDate;
            existingEntry.Description = entryVm.JournalEntry.Description;
            existingEntry.IsPosted = entryVm.JournalEntry.IsPosted;
            
            // Add new details
            foreach (var detail in entryVm.JournalEntry.Details)
            {
                existingEntry.Details.Add(detail);
            }

            await _unitOfWork.JournalEntry.Update(existingEntry);
            entryVm.JournalEntry = existingEntry;
        }

        await _unitOfWork.SaveAsync();

        if (entryVm.JournalEntry.IsPosted)
        {
            await PostEntry(entryVm.JournalEntry);
        }

        TempData["success"] = $"Journal entry {(entryVm.JournalEntry.Id == 0 ? "created" : "updated")} successfully";
        return RedirectToAction("Index", "Journal");
    }

    private async Task<string> GenerateEntryNumber(int journalId)
    {
        var journal = await _unitOfWork.Journal.Get(j => j.Id == journalId);
        var entriesCount = await _unitOfWork.JournalEntry.GetAll(je => je.JournalId == journalId);
        return $"{journal?.Type.ToString()[..3]}-{DateTime.Now:yyyyMM}-{entriesCount.Count() + 1:D4}";
    }

    private async Task<List<SelectListItem>> GetActiveAccounts()
    {
        var accounts = await _unitOfWork.Account.GetAll(a => a.IsActive);
        return accounts.Select(a => new SelectListItem
        {
            Text = $"{a.Id} - {a.Name}",
            Value = a.Id.ToString()
        }).ToList();
    }

    private async Task PostEntry(JournalEntry entry)
    {
        foreach (var detail in entry.Details)
        {
            var account = await _unitOfWork.Account.Get(a => a.Id == detail.AccountId);
            if (account == null) continue;

            // Update account balance based on account type and debit/credit amounts
            switch (account.Type)
            {
                case "Asset" or "Expense":
                    account.Balance += detail.DebitAmount - detail.CreditAmount;
                    break;
                case "Liability" or "Equity" or "Revenue":
                    account.Balance += detail.CreditAmount - detail.DebitAmount;
                    break;
            }

            await _unitOfWork.Account.Update(account);
        }

        entry.PostedDate = DateTime.Now;
        entry.PostedBy = User.Identity?.Name!;
        await _unitOfWork.SaveAsync();
    }*/
}