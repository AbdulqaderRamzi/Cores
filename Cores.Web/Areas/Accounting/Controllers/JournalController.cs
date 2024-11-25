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
public class JournalController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public JournalController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var journals = await _unitOfWork.Journal.GetAll();
        return View(journals);
    }

    public async Task<IActionResult> Upsert(int? id)
    {
        var journalVm = new JournalVm
        {
            Journal = new(),
        };
        FillSelectionData(journalVm);

        if (id is null or 0)
        {
            return View(journalVm);
        }

        var journal = await _unitOfWork.Journal.Get(j => j.Id == id);
        if (journal is null)
        {
            return NotFound();
        }

        journalVm.Journal = journal;
        return View(journalVm);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(JournalVm journalVm)
    {
        if (!ModelState.IsValid)
        {
           FillSelectionData(journalVm);
            return View(journalVm);
        }

        if (journalVm.Journal.Id == 0)
        {
            journalVm.Journal.CreatedAt = DateTime.Now;
            journalVm.Journal.CreatedBy = User.Identity?.Name!;
            await _unitOfWork.Journal.Add(journalVm.Journal);
            TempData["success"] = "Journal created successfully";
        }
        else
        {
            await _unitOfWork.Journal.Update(journalVm.Journal);
            TempData["success"] = "Journal updated successfully";
        }

        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
    
    private static void FillSelectionData(JournalVm journalVm)
    {
        journalVm.JournalTypes = Enum.GetValues(typeof(JournalType))
            .Cast<JournalType>()
            .Select(t => new SelectListItem
            {
                Text = t.ToString(),
                Value = t.ToString()
            }).ToList();
    }

    #region API CALLS
    [HttpGet]
    public async Task<IActionResult> GetJournalEntries(int journalId)
    {
        var entries = await _unitOfWork.JournalEntry.GetAll(
            e => e.JournalId == journalId,
            includeProperties: "Details.Account");
        return Json(new { data = entries });
    }
    #endregion
    
 
}