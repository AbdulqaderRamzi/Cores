using System.Security.Claims;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Web.Areas.HR.Controllers;

[Area("HR")]
[Authorize(Roles = SD.HR_ROLE + "," + SD.ADMIN_ROLE)]
public class HolidayTypeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public HolidayTypeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var holidayTypes = await _unitOfWork.HolidayType.GetAll();
        return View(holidayTypes);
    }

    public async Task<IActionResult> Upsert(int id)
    {
        if (id is 0)
        {
            var holidayType = new HolidayType
            {
                CreatedAt = DateTime.Now
            };
            return View(holidayType);
        }

        var holidayTypeFromDb = await _unitOfWork.HolidayType.Get(ht => ht.Id == id);
        if (holidayTypeFromDb == null)
            return NotFound();
            
        return View(holidayTypeFromDb);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(HolidayType holidayType)
    {
        if (!ModelState.IsValid)
            return View(holidayType);

        if (holidayType.Id is 0)
        {
            await _unitOfWork.HolidayType.Add(holidayType);
            TempData["success"] = "Holiday type created successfully";
        }
        else
        {
            await _unitOfWork.HolidayType.Update(holidayType);
            TempData["success"] = "Holiday type updated successfully";
        }

        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
            return BadRequest();
            
        var holidayType = await _unitOfWork.HolidayType.Get(ht => ht.Id == id);
        if (holidayType == null)
            return NotFound();
            
        _unitOfWork.HolidayType.Remove(holidayType);
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
}