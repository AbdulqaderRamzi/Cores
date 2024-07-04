using Cores.DataService.Repository.IRepository;
using Cores.Models.CRM;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.CRM.Controllers;

[Area("CRM")]
[Authorize(Roles = SD.CRM_ROLE + "," + SD.ADMIN_ROLE)]
public class EventTypeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public EventTypeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var eventTypes = await _unitOfWork.EventType.GetAll();
        return View(eventTypes);
    }

    public async Task<IActionResult> Upsert(int id)
    {
        if (id is 0)
        {
            return View(new EventType());
        }

        var eventType = await _unitOfWork.EventType.Get(et => et.Id == id);
        if (eventType is null)
            return NotFound();
        return View(eventType);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(EventType eventType)
    {
        if (!ModelState.IsValid)
            return View(eventType);
        if (eventType.Id is 0)
        {
            await _unitOfWork.EventType.Add(eventType);
            TempData["success"] = "Event Type added successfully";
        }
        else
        {
            await _unitOfWork.EventType.Update(eventType);
            TempData["success"] = "Event Type updated successfully";
        }

        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        if (id is 0)
            return BadRequest();
        var eventType = await _unitOfWork.EventType.Get(pt => pt.Id == id);
        if (eventType is null)
            return NotFound();
        _unitOfWork.EventType.Remove(eventType);
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
}