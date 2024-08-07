﻿using System.Security.Claims;
using Cores.DataService.Repository.IRepository;
using Cores.Models.HR;
using Cores.Models.ViewModels;
using Cores.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Web.Areas.HR.Controllers;

[Area("HR")]
[Authorize(Roles = SD.HR_ROLE + "," + SD.ADMIN_ROLE + "," + SD.EMPLOYEE_ROLE)]
public class AttendanceController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public AttendanceController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<IActionResult> HrAttendance()
    {
        var attendances = await _unitOfWork.Attendance.GetAll(includeProperties: "Employee");
        return View(nameof(Index), attendances);
    }

    public async Task<IActionResult> Upsert(int id)
    {
        var attendanceVm = new AttendanceVm
        {
            Attendance = new()
            {
                Date = DateOnly.FromDateTime(DateTime.Now)
            }
        };
        var employees = await _unitOfWork.ApplicationUser.GetAll();
        attendanceVm.Employees = employees.Select(e => new SelectListItem
        {  
           Text = $"{e.FirstName} {e.LastName}",
           Value = e.Id
        }).ToList();

        if (id is 0)
        {
            return View(attendanceVm);
        }

        var attendance = await _unitOfWork.Attendance.Get(a => a.Id == id);
        if (attendance is null)
            return NotFound();
        
        attendanceVm.Attendance = attendance;
        return View(attendanceVm);

    }
    
    [HttpPost]
    public async Task<IActionResult> Upsert(AttendanceVm attendanceVm)
    {
        if (!ModelState.IsValid)
        { 
            var employees = await _unitOfWork.ApplicationUser.GetAll();
            attendanceVm.Employees = employees.Select(e => new SelectListItem
            {  
                Text = $"{e.FirstName} {e.LastName}",
                Value = e.Id
            }).ToList();
            return View(attendanceVm);
        }

        if (attendanceVm.Attendance.Id is 0)
        {
            await _unitOfWork.Attendance.Add(attendanceVm.Attendance);
            TempData["success"] = "Attendance added successfully";
        }
        else
        {
            await _unitOfWork.Attendance.Update(attendanceVm.Attendance);
            TempData["success"] = "Attendance updated successfully";
        }

        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(HrAttendance));
    }

    public async Task<IActionResult> Delete(int id)
    {
        if (id is 0)
            return BadRequest();
        var attendance = await _unitOfWork.Attendance.Get(a => a.Id == id);
        if (attendance is null)
            return NotFound();
        _unitOfWork.Attendance.Remove(attendance);
        await _unitOfWork.SaveAsync();
        return RedirectToAction(nameof(HrAttendance));
    }

    public async Task<IActionResult> EmployeeAttendance()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity!;
        var employeeId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var attendances = await _unitOfWork.Attendance.GetAll(a => a.EmployeeId == employeeId, includeProperties: "Employee");
        return View(nameof(Index), attendances);
    }
    
    public IActionResult Sign()
    {
        var attendance = new Attendance();
        var claimsIdentity = (ClaimsIdentity)User.Identity!;
        var employeeId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        attendance.EmployeeId = employeeId;
        return View(attendance);
    }

    public async Task<IActionResult> SignIn(Attendance attendance)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity!;
        var employeeId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var employeeAttendance = await _unitOfWork.Attendance.Get(
            a => a.Date == DateOnly.FromDateTime(DateTime.Now)
             && a.EmployeeId == employeeId
             );
        if (employeeAttendance is null)
        {
            attendance.Date = DateOnly.FromDateTime(DateTime.Now);
            attendance.TimeIn = TimeOnly.FromDateTime(DateTime.Now);
            await _unitOfWork.Attendance.Add(attendance);
            await _unitOfWork.SaveAsync();
            TempData["success"] = "You signed in successfully";
            return RedirectToAction(nameof(EmployeeAttendance));
        }
        TempData["error"] = "You already signed in!";
        return RedirectToAction(nameof(EmployeeAttendance));

        
    }   
    
    public async Task<IActionResult> SignOut(Attendance attendance)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity!;
        var employeeId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var employeeAttendance = await _unitOfWork.Attendance.Get(
            a => a.Date == DateOnly.FromDateTime(DateTime.Now)
                 && a.EmployeeId == employeeId
        );

        if (employeeAttendance is not null && employeeAttendance.TimeOut is  null)
        {
            employeeAttendance.TimeOut = TimeOnly.FromDateTime(DateTime.Now);
            await _unitOfWork.Attendance.Update(employeeAttendance);
            await _unitOfWork.SaveAsync();
            TempData["success"] = "You signed out successfully";
            return RedirectToAction(nameof(EmployeeAttendance));

        }
        TempData["error"] = "You already signed out!";
        
        return RedirectToAction(nameof(EmployeeAttendance));
    }
}