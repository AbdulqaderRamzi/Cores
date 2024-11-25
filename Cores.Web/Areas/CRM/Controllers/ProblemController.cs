using System.Security.Claims;
using Cores.DataService.Repository.IRepository;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Cores.Models.CRM;
using Cores.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Web.Areas.CRM.Controllers;

[Area("CRM")]
[Authorize(Roles = SD.CRM_ROLE + "," + SD.ADMIN_ROLE)]
public class ProblemController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public ProblemController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<IActionResult> Index()
    {
        
        var problems = await _unitOfWork.Problem.GetAll(includeProperties:"Contact,ApplicationUser,ProblemType");
        return View(problems);
    }
    
    public async Task<IActionResult> Upsert(int id)
    {
        var problem = new Problem();
        if (id is not 0)
        {
            problem = await _unitOfWork.Problem.Get(p => p.Id == id, includeProperties:"ApplicationUser,ModifiedBy");
            if (problem is null)
                return NotFound();
        }
        else
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            problem.ApplicationUserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var employee = await _unitOfWork.ApplicationUser.Get(a => a.Id == problem.ApplicationUserId);
            if (employee is null)
                return NotFound();
            problem.ApplicationUser = employee;   
        }

        var contacts = await _unitOfWork.Contact.GetAll();
        var contactListItems = contacts.Select(c => new SelectListItem
        {   
            Text = string.Concat(c.FirstName, " ", c.LastName),
            Value = c.Id.ToString()
        }).ToList();
        
        var problemTypes = await _unitOfWork.ProblemType.GetAll();
        var problemTypesListItems = problemTypes.Select(pt => new SelectListItem
        {   
            Text = pt.Type,
            Value = pt.Id.ToString()
        }).ToList();
       
        var problemVm = new ProblemVm
        {
            Problem = problem, 
            Contacts = contactListItems,
            ProblemTypes = problemTypesListItems
        };
        return View(problemVm);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(ProblemVm problemVm)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }

        if (problemVm.Problem.Id is 0)
        {
            await _unitOfWork.Problem.Add(problemVm.Problem);
            TempData["success"] = "Problem added successfully.";
        }
        else
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            problemVm.Problem.ModifiedById = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _unitOfWork.Problem.Update(problemVm.Problem);
            TempData["success"] = "Problem updated successfully.";
        }
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));

    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
            return BadRequest();
        var problem = await _unitOfWork.Problem.Get(p => p.Id == id);
        if (problem is null)
            return NotFound();
        _unitOfWork.Problem.Remove(problem);
        await _unitOfWork.SaveAsync();
        TempData["success"] = "Problem deleted successfully";
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