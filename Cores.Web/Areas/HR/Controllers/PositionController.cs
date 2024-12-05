using Cores.DataService.Repository.IRepository;
using Cores.Models.ViewModels;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Web.Areas.HR.Controllers;


[Area("HR")]
[Authorize(Roles = SD.CRM_ROLE + "," + SD.ADMIN_ROLE)]
public class PositionController : Controller
{
   private readonly IUnitOfWork _unitOfWork;

   public PositionController(IUnitOfWork unitOfWork)
   {
      _unitOfWork = unitOfWork;
   }

   public async Task<IActionResult> Index()
   {
      var position = await _unitOfWork.Position.GetAll(includeProperties:"Department,Currency");
      return View(position);
   }

   public async Task<IActionResult> Upsert(int id)
   {
      var positionVm = new PositionVm();
      positionVm.Position = new();

      await FillSelectionDate(positionVm);
      if (id is 0)
      {
         return View(positionVm);
      }

      var position = await _unitOfWork.Position.Get(p => p.Id == id, includeProperties:"Employees,Department");
      if (position is null)
         return NotFound();
      positionVm.Position = position;
      return View(positionVm);
   }

   [HttpPost]
   public async Task<IActionResult> Upsert(PositionVm positionVm)
   {
      if (!ModelState.IsValid)
      {
         await FillSelectionDate(positionVm);
         return View(positionVm);
      }

      if (positionVm.Position.Id is 0)
      {
         await _unitOfWork.Position.Add(positionVm.Position);
         TempData["success"] = "Position added successfully";
      }
      else
      {
         await _unitOfWork.Position.Update(positionVm.Position);
         TempData["success"] = "Position updated successfully";
      }

      await _unitOfWork.SaveAsync();
      return RedirectToAction(nameof(Index));
   }
   
   public async Task<IActionResult> Delete(int? id)
   {
      if (id is null)
         return NotFound();
      var position = await _unitOfWork.Position.Get(p => p.Id == id, includeProperties:"Employees");
      if (position is null)
         return NotFound();
      if (position.Employees.Count is not 0)
      {
         TempData["error"] = "The position has employees";
         return RedirectToAction(nameof(Index));
      }
      _unitOfWork.Position.Remove(position);
      await _unitOfWork.SaveAsync();
      TempData["success"] = "Position deleted successfully";
      return RedirectToAction(nameof(Index));
   }

   private async Task FillSelectionDate(PositionVm vm)
   {
      var departments = await _unitOfWork.Department.GetAll();
      var currencies = await _unitOfWork.Currency.GetAll();
      vm.Departments = departments.Select(d => new SelectListItem
      {
         Text = d.Name,
         Value = d.Id.ToString()
      }).ToList();
      
      vm.Currencies = currencies.Select(d => new SelectListItem
      {
         Text = $"{d.Code} - {d.Name}",
         Value = d.Id.ToString()
      }).ToList();
   }

   #region API CALL
   
   [HttpGet]
   public async Task<IActionResult> GetPositionById(int? id)
   {
      
      var position = await _unitOfWork.Position.Get(p => p.Id == id);
      
      return Json(position);
   }

   #endregion
   
}