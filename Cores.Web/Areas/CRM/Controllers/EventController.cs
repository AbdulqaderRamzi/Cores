using System.Security.Claims;
using Cores.DataService.Repository.IRepository;
using Cores.Models.ViewModels;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Web.Areas.CRM.Controllers;

[Area("CRM")]
[Authorize(Roles = SD.CRM_ROLE + "," + SD.ADMIN_ROLE)]
public class EventController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public EventController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var events = await _unitOfWork.Event.GetAll(includeProperties: "EventType,Contact,ModifiedBy,ApplicationUser");
        return View(events);
    }

    public async Task<IActionResult> Upsert(int id)
    {
        var eventVm = new EventVm();
        
        var eventTypes = await _unitOfWork.EventType.GetAll();
        eventVm.EventTypeList = eventTypes.Select(et => new SelectListItem
        {
            Text = et.Type,
            Value = et.Id.ToString()
        }).ToList();
        
        var contacts = await _unitOfWork.Contact.GetAll();
        eventVm.Contacts = contacts.Select(c => new SelectListItem
        {
            Text = string.Concat(c.FirstName, " ", c.LastName),
            Value = c.Id.ToString()
        }).ToList();

        
        if (id is 0)
        {
            eventVm.Event = new();
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            eventVm.Event.ApplicationUserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var employee = await _unitOfWork.ApplicationUser.Get(a => a.Id == eventVm.Event.ApplicationUserId);
            if (employee is null)
                return NotFound();
            eventVm.Event.ApplicationUser = employee;
            eventVm.Event.DateTime = DateTime.Now;
            return View(eventVm);
        }

        var @event = await _unitOfWork.Event.Get(e => e.Id == id, includeProperties:"Contact,EventType,ApplicationUser,ModifiedBy");
        if (@event is null)
            return NotFound();
        eventVm.Event = @event;
        return View(eventVm);

    }

    [HttpPost]
    public async Task<IActionResult> Upsert(EventVm eventVm)
    {
        if (!ModelState.IsValid)
        {
            var eventTypes = await _unitOfWork.EventType.GetAll();
            eventVm.EventTypeList = eventTypes.Select(et => new SelectListItem
            {
                Text = et.Type,
                Value = et.Id.ToString()
            }).ToList();
            return View(eventVm);
        }

        if (eventVm.Event.Id is 0)
        {
            await _unitOfWork.Event.Add(eventVm.Event);
            TempData["success"] = "Event added successfully";
        }
        else
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            eventVm.Event.ModifiedById = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            await _unitOfWork.Event.Update(eventVm.Event);
            TempData["success"] = "Event updated successfully";
        }

        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
            return BadRequest();
        var @event = await _unitOfWork.Event.Get(e => e.Id == id);
        if (@event is null)
            return NotFound();
        _unitOfWork.Event.Remove(@event);
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
    
    #region API CALL
    
    [HttpGet]
    public async Task<IActionResult> GetContactById(int id)
    {
        var contact = await _unitOfWork.Contact.Get(c => c.Id == id);
        return Json(contact);
    }

    #endregion
    
}