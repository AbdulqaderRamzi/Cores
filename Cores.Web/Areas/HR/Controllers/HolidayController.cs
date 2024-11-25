using System.Security.Claims;
using Cores.DataService.Repository.IRepository;
using Cores.Models.ViewModels;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Web.Areas.HR.Controllers;

[Area("HR")]
[Authorize(Roles = SD.HR_ROLE + "," + SD.ADMIN_ROLE)]
public class HolidayController : Controller
{
   private readonly IUnitOfWork _unitOfWork;

    public HolidayController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var holidays = await _unitOfWork.Holiday.GetAll(includeProperties: "HolidayType");
        return View(holidays);
    }

    public async Task<IActionResult> Upsert(int id)
    {
        var holidayVm = new HolidayVm
        {
            Holiday = new()
        };
        await FillSelectionData(holidayVm);

        if (id is 0)
        {
            return View(holidayVm);
        }

        var holiday = await _unitOfWork.Holiday.Get(h => h.Id == id, includeProperties: "HolidayType");
        if (holiday is null)
            return NotFound();
        holidayVm.Holiday = holiday;
        return View(holidayVm);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(HolidayVm holidayVm)
    {
        if (!ModelState.IsValid)
        {
            await FillSelectionData(holidayVm);
            return View(holidayVm);
        }

        if (holidayVm.Holiday.Id is 0)
        {
            holidayVm.Holiday.CreatedAt = DateTime.Now;
            await _unitOfWork.Holiday.Add(holidayVm.Holiday);
            TempData["success"] = "Holiday created successfully";
        }
        else
        {
            await _unitOfWork.Holiday.Update(holidayVm.Holiday);
            TempData["success"] = "Holiday updated successfully";
        }

        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
            return BadRequest();
            
        var holiday = await _unitOfWork.Holiday.Get(h => h.Id == id);
        if (holiday == null)
            return NotFound();
            
        _unitOfWork.Holiday.Remove(holiday);
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task FillSelectionData(HolidayVm holidayVm)
    {
        holidayVm.HolidayTypes = (await _unitOfWork.HolidayType.GetAll())
            .Select(ht => new SelectListItem
            {
                Text = ht.Name,
                Value = ht.Id.ToString()
            });
    }
}