using System.Security.Claims;
using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
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
        var journalEntries = await _unitOfWork.JournalEntry.GetAll(includeProperties: "Journal.JournalType,Account,ApplicationUser");
        return View(journalEntries);
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
    
        var journalEntryVm = new JournalEntryVm
        {
            JournalEntry = new JournalEntry
            {
                ApplicationUserId = employeeId
            },
            EmployeeName = $"{employee.FirstName} {employee.LastName}"
        };

        await FillSelectionData(journalEntryVm);
        
        if (id is 0)
            return View(journalEntryVm);

        var journalEntry = await _unitOfWork.JournalEntry.Get(je => je.Id == id);
        if (journalEntry is null)
            return NotFound();
        journalEntryVm.JournalEntry = journalEntry;
        
        return View(journalEntryVm);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(JournalEntryVm journalEntryVm)
    {
        if (!ModelState.IsValid)
        {
            await FillSelectionData(journalEntryVm);
            return View(journalEntryVm);
        }
        
        if (journalEntryVm.JournalEntry.Id == 0)
        {
            await _unitOfWork.JournalEntry.Add(journalEntryVm.JournalEntry);
            TempData["success"] = "Journal entry created successfully";
        }
        else
        {
            journalEntryVm.JournalEntry.UpdatedAt = DateTime.Now;
            await _unitOfWork.JournalEntry.Update(journalEntryVm.JournalEntry);
            TempData["success"] = "Journal entry updated successfully";
        }
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
            return NotFound();
        var journalEntry = await _unitOfWork.JournalEntry.Get(jt => jt.Id == id);
        if (journalEntry is null)
            return NotFound();
        _unitOfWork.JournalEntry.Remove(journalEntry);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Journal Entry deleted successfully";
        return RedirectToAction(nameof(Index));
    }

    private async Task FillSelectionData(JournalEntryVm journalEntryVm)
    {
        var accounts = await _unitOfWork.Account.GetAll();
        journalEntryVm.Accounts = accounts.Select(a => new SelectListItem
        {
            Text = a.Name,
            Value = a.Id.ToString()
        }).ToList();
        
        var journals = await _unitOfWork.Journal.GetAll(includeProperties:"JournalType");
        journalEntryVm.Journals = journals.Select(j => new SelectListItem
        {
            Text = $"{j.Id.ToString()}. {j.JournalType.Type}",
            Value = j.Id.ToString()
        }).ToList();
    }
}