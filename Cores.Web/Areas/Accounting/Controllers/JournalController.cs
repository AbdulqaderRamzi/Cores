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
public class JournalController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public JournalController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var journals = await _unitOfWork.Journal.GetAll(includeProperties: "JournalType");
        return View(journals);
    }

    public async Task<IActionResult> Upsert(int id)
    {
        var journalVm = new JournalVm
        {
            Journal = new Journal(),
        };
        await FillSelectionData(journalVm);

        if (id is 0)
            return View(journalVm);

        var journal = await _unitOfWork.Journal.Get(j => j.Id == id, includeProperties: "JournalType");
        if (journal == null)
            return NotFound();
        journalVm.Journal = journal;
        return View(journalVm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upsert(JournalVm journalVm)
    {
        if (!ModelState.IsValid)
        {
            await FillSelectionData(journalVm);
            return View(journalVm);
        }

        if (journalVm.Journal.Id == 0)
        {
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
    
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
            return NotFound();
        var journal = await _unitOfWork.Journal.Get(j => j.Id == id);
        if (journal is null)
            return NotFound();
        _unitOfWork.Journal.Remove(journal);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Journal deleted successfully";
        return RedirectToAction(nameof(Index));
    }
    
    private async Task FillSelectionData(JournalVm journalVm)
    {
        var journalTypes = await _unitOfWork.JournalType.GetAll();
        journalVm.JournalTypes = journalTypes.Select(j => new SelectListItem
        {
            Text = j.Type,
            Value = j.Id.ToString()
        }).ToList();
    }
}