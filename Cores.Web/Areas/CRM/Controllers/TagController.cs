using Cores.DataService.Repository.IRepository;
using Cores.Models;
using Cores.Models.CRM;
using Cores.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.CRM.Controllers;

[Area("CRM")]
[Authorize(Roles = SD.CRM_ROLE + "," + SD.ADMIN_ROLE)]
public class TagController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public TagController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    public async Task<IActionResult> Index()
    {
        var tags = await _unitOfWork.Tag.GetAll();
        return View(tags);
    }

    public async Task<IActionResult> Upsert(int? id)
    {
        if (id is null or 0)
            return View(new Tag());
        var tag = await _unitOfWork.Tag.Get(u => u.Id == id);
        return View(tag);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(Tag tag)
    {
        if (!ModelState.IsValid)
            return View(tag);
        if (tag.Id is 0)
            await _unitOfWork.Tag.Add(tag);
        else
            await _unitOfWork.Tag.Update(tag);
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
            return NotFound();
        var tag = await _unitOfWork.Tag.Get(t => t.Id == id);
        if (tag is null)
            return NotFound();
        _unitOfWork.Tag.Remove(tag);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Tag deleted successfully";
        return RedirectToAction(nameof(Index));
    }
}