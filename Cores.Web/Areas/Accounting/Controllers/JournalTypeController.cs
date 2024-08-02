using Cores.DataService.Repository.IRepository;
using Cores.Models.Accounting;
using Cores.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.Accounting.Controllers;

[Area("Accounting")]
[Authorize(Roles = SD.ACCOUNTING_ROLE + "," + SD.ADMIN_ROLE)]
public class JournalTypeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public JournalTypeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var journalTypes = await _unitOfWork.JournalType.GetAll();
        return View(journalTypes);
    }

    public async Task<IActionResult> Upsert(int id)
    {
        if (id is 0)
            return View(new JournalType());

        var journalType = await _unitOfWork.JournalType.Get(jt => jt.Id == id);
        if (journalType == null)
            return NotFound();

        return View(journalType);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(JournalType journalType)
    {
        if (!ModelState.IsValid)
            return View(journalType);

        if (journalType.Id == 0)
        {
            await _unitOfWork.JournalType.Add(journalType);
            TempData["success"] = "Journal Type created successfully";
        }
        else
        {
            await _unitOfWork.JournalType.Update(journalType);
            TempData["success"] = "Journal Type updated successfully";
        }
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Delete(int id)
    {
        if (id is 0)
            return BadRequest();
        var journalType = await _unitOfWork.JournalType.Get(jt => jt.Id == id);
        if (journalType is null)
            return NotFound();
        _unitOfWork.JournalType.Remove(journalType);
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
}