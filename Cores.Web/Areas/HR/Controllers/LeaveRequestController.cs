using Cores.DataService.Repository.IRepository;
using Cores.Models.ViewModels;
using Cores.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Cores.Models;
using Cores.Models.HR;

namespace Cores.Web.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize]
    public class LeaveRequestController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LeaveRequestController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var leaveRequests = await _unitOfWork.LeaveRequest.GetAll(includeProperties: "Employee,LeaveType");
            return View(leaveRequests);
        }

        public async Task<IActionResult> WaitingRequests(bool isFromManager = false)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = await _unitOfWork.ApplicationUser.Get(e => e.Id == userId);
            if (employee is null)
            {
                return NotFound();
            }

            IEnumerable<LeaveRequest> leaveRequests;

            if (isFromManager) // this may come from hr or employee manager
            {
                if (employee.IsManager)
                {
                    leaveRequests = await _unitOfWork.LeaveRequest.GetAll(lr => lr.ManagerResponse == null
                        && lr.ManagerId == employee.Id, includeProperties: "Employee,LeaveType");
                }
                else
                {
                    return Forbid();
                }
            }
            else // this must appears to all hr employee after the manager accept the request
            {
                leaveRequests = await _unitOfWork.LeaveRequest.GetAll(
                    lr => lr.ManagerResponse == true && lr.HrResponse == null,
                    includeProperties: "Employee,LeaveType");
            }

            ViewBag.IsFromManager = isFromManager;
            return View(leaveRequests);

        }

        public async Task<IActionResult> Accept(int id, bool isManager)
        {
            var leaveRequest = await _unitOfWork.LeaveRequest.Get(lr => lr.Id == id);
            if (leaveRequest is not null)
            {
                leaveRequest.LeaveStatus = SD.ACCEPT;
                if (isManager)
                {
                    leaveRequest.ManagerResponse = true;
                    leaveRequest.ManagerResponseDate = DateTime.Now;
                }
                else
                {
                    var employee = await _unitOfWork.ApplicationUser.Get(e => e.Id == leaveRequest.EmployeeId, includeProperties:"WorkSchedules");
                    if (employee is null)
                        return NotFound();
                    /*if (leaveRequest.IsPayed)
                    {
                        var employeeLeaveBalance = await _unitOfWork.EmployeeLeaveBalance.Get(
                            elb => elb.EmployeeId == leaveRequest.EmployeeId &&
                                   elb.LeaveTypeId == leaveRequest.LeaveTypeId);
                        employee.AnnualLeaveBalance -= leaveRequest.NumberOfDays;
                        employeeLeaveBalance!.DaysUsed += leaveRequest.NumberOfDays;
                    }
                    else
                    {
                        var workingDays = employee.WorkingDaysInMonth;
                        if (workingDays is not null and not 0)
                        {
                            var dailyRate = employee.Salary / workingDays;
                            var deduction = dailyRate * leaveRequest.NumberOfDays;
                            var unpaidLeaveDeduction = new UnpaidLeaveDeduction
                            {
                                Deduction = (decimal) deduction!,
                                DateTime = DateTime.Now,
                                ApplicationUserId = employee.Id
                            };
                            await _unitOfWork.UnpaidLeaveDeduction.Add(unpaidLeaveDeduction);
                        }
                    }*/

                    leaveRequest.HrResponse = true;
                    leaveRequest.HrResponseDate = DateTime.Now;
                }
            }
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(WaitingRequests), new { isFromManager = isManager });
        }

        public async Task<IActionResult> Reject(int id, string responseReason, bool isManager)
        {
            var leaveRequest = await _unitOfWork.LeaveRequest.Get(lr => lr.Id == id);
            if (leaveRequest is not null)
            {
                leaveRequest.LeaveStatus = SD.REJECT;
                leaveRequest.ResponseReason = responseReason;
                if (isManager)
                {
                    leaveRequest.ManagerResponse = false;
                    leaveRequest.ManagerResponseDate = DateTime.Now;
                }
                else
                {
                    leaveRequest.HrResponse = false;
                    leaveRequest.HrResponseDate = DateTime.Now;
                }
            }

            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(WaitingRequests), new { isFromManager = isManager });
        }

        /*public async Task<IActionResult> Cancel(int? id)
        {
            var leaveRequest = await _unitOfWork.LeaveRequest.Get(lr => lr.Id == id);
            if (leaveRequest is null)
            {
                TempData["error"] = "An error Occured";
                return RedirectToAction(nameof(GetById));
            }
            
            var employee = await _unitOfWork.ApplicationUser.Get(e => e.Id == leaveRequest.EmployeeId);
            var employeeLeaveBalance = await _unitOfWork.EmployeeLeaveBalance.Get(
                elb => elb.EmployeeId == leaveRequest.EmployeeId &&
                       elb.LeaveTypeId == leaveRequest.LeaveTypeId);
            if (employee is not null && employeeLeaveBalance is not null)
            {
                employee.AnnualLeaveBalance += leaveRequest.NumberOfDays;
                employeeLeaveBalance.DaysUsed -= Math.Min(0, leaveRequest.NumberOfDays);
            }

            _unitOfWork.LeaveRequest.Remove(leaveRequest);
            await _unitOfWork.SaveAsync();
            TempData["success"] = $"The Request Canceled Successfully";
            return RedirectToAction(nameof(GetById));
        }*/


        public async Task<IActionResult> GetById(string id)
        {
            var leaveRequests = 
                await _unitOfWork.LeaveRequest.GetAll(lr => lr.EmployeeId == id, includeProperties: "Employee,LeaveType");
            ViewBag.IsFromGetById = true;
            return View(nameof(Index), leaveRequests);
        }

        public async Task<IActionResult> Upsert(int id, bool? isFromGetById)
        {
            var leaveRequestVm = new LeaveRequestVm
            {
                LeaveRequest = new()
            };

            if (isFromGetById is not null)
            {
                var employeeId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var employee = await _unitOfWork.ApplicationUser.Get(e => e.Id == employeeId);
                
                if (employee is not null)
                { 
                    var fullName = $"{employee.FirstName} {employee.LastName}";
                    leaveRequestVm.EmployeeName = fullName;
                    leaveRequestVm.LeaveRequest.EmployeeId = employeeId;
                }
            }

            await FillSelectionData(leaveRequestVm);

            if (id == 0)
            {
                return View(leaveRequestVm);
            }

            var leaveRequest = await _unitOfWork.LeaveRequest.Get(lr => lr.Id == id, includeProperties: "Employee,LeaveType");
            if (leaveRequest == null)
                return NotFound();
            leaveRequestVm.LeaveRequest = leaveRequest;
          

            return View(leaveRequestVm);
        }

      [HttpPost]
        public async Task<IActionResult> Upsert(LeaveRequestVm leaveRequestVm, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                await FillSelectionData(leaveRequestVm);
                return View(leaveRequestVm);
            }
            
            var employee = await _unitOfWork.ApplicationUser.Get(e => e.Id == leaveRequestVm.LeaveRequest.EmployeeId);
            if (employee is null)
            {
                ModelState.AddModelError("", "Employee not found");
                await FillSelectionData(leaveRequestVm);
                return View(leaveRequestVm);
            }
            
            var leaveType = await _unitOfWork.LeaveType.Get(lt => lt.Id == leaveRequestVm.LeaveRequest.LeaveTypeId);
            if (leaveType is null)
            {
                ModelState.AddModelError("", "Leave Type not found");
                await FillSelectionData(leaveRequestVm);
                return View(leaveRequestVm);
            }

            if (leaveRequestVm.LeaveRequest.NumberOfDays <= 0)
            {
                ModelState.AddModelError("", "End date must be after start date");
                await FillSelectionData(leaveRequestVm);
                return View(leaveRequestVm);
            }

            var startDate = leaveRequestVm.LeaveRequest.StartDate;
            var noOfDays = leaveRequestVm.LeaveRequest.NumberOfDays;
            if (leaveRequestVm.LeaveRequest.IsPayed && !CanApproveLeaveRequest(employee, startDate, noOfDays ) /*employee.AnnualLeaveBalance < leaveRequestVm.LeaveRequest.NumberOfDays*/)
            {
                ModelState.AddModelError("", "Insufficient leave balance");
                await FillSelectionData(leaveRequestVm);
                return View(leaveRequestVm);
            }
            
            var employeeLeaveBalance = await _unitOfWork.EmployeeLeaveBalance.Get(
                elb => elb.EmployeeId == leaveRequestVm.LeaveRequest.EmployeeId && 
                       elb.LeaveTypeId == leaveRequestVm.LeaveRequest.LeaveTypeId);
            if (leaveRequestVm.LeaveRequest.IsPayed)
            {
                if (employeeLeaveBalance is null)
                {
                    employeeLeaveBalance = new EmployeeLeaveBalance
                    {
                        EmployeeId = leaveRequestVm.LeaveRequest.EmployeeId,
                        LeaveTypeId = leaveRequestVm.LeaveRequest.LeaveTypeId,
                        EndDate = DateTime.Now.AddYears(1),
                        CurrentDate = DateTime.Now
                    };
                    await _unitOfWork.EmployeeLeaveBalance.Add(employeeLeaveBalance);
                }
                else
                {
                    employeeLeaveBalance.CurrentDate = DateTime.Now;
                }

                if (employeeLeaveBalance.CurrentDate > employeeLeaveBalance.EndDate)
                {
                    employeeLeaveBalance.DaysUsed = 0;
                    employeeLeaveBalance.CurrentDate = DateTime.Now;
                    employeeLeaveBalance.EndDate = DateTime.Now.AddYears(1);
                    await _unitOfWork.SaveAsync();
                }

                var isValid = employeeLeaveBalance.DaysUsed + leaveRequestVm.LeaveRequest.NumberOfDays <=
                              leaveType.MaxDaysPerYear;
                if (!isValid)
                {
                    ModelState.AddModelError("", "Leave request exceeds the maximum allowed days for this leave type");
                    await FillSelectionData(leaveRequestVm);
                    return View(leaveRequestVm);
                }
            }

            var wwwRootPath = _webHostEnvironment.WebRootPath;
            if (file is not null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var leavePath = Path.Combine(wwwRootPath, @"documents\leave_requests");
                
                if (!Directory.Exists(leavePath))
                    Directory.CreateDirectory(leavePath);

                await using (var fileStream = new FileStream(Path.Combine(leavePath, fileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                if (!string.IsNullOrEmpty(leaveRequestVm.LeaveRequest.Document))
                {
                    var oldFilePath = Path.Combine(wwwRootPath, leaveRequestVm.LeaveRequest.Document.TrimStart('\\'));
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                leaveRequestVm.LeaveRequest.Document = @"\documents\leave_requests\" + fileName;
            }
            else
            {
                var id = leaveRequestVm.LeaveRequest.Id;
                var leaveRequestFromDb = await _unitOfWork.LeaveRequest.Get(lr => lr.Id == id);
                if (leaveRequestFromDb is not null)
                {
                    leaveRequestVm.LeaveRequest.Document = leaveRequestFromDb.Document;
                }

            }

            if (leaveRequestVm.LeaveRequest.Id is 0)
            {
                await _unitOfWork.LeaveRequest.Add(leaveRequestVm.LeaveRequest);
                leaveRequestVm.LeaveRequest.ManagerId = employee.ManagerID;
                TempData["success"] = "Leave request submitted successfully";
            }
            else
            {
                await _unitOfWork.LeaveRequest.Update(leaveRequestVm.LeaveRequest);
                TempData["success"] = "Leave request updated successfully";
            }

            await _unitOfWork.SaveAsync();
            return leaveRequestVm.EmployeeName is null ? RedirectToAction(nameof(Index)) 
                : RedirectToAction(nameof(GetById), new { id = leaveRequestVm.LeaveRequest.EmployeeId });
        }
        
        public bool CanApproveLeaveRequest(ApplicationUser employee, DateTime leaveStartDate, int requestedLeaveDays)
        {
            if (employee is not { AnnualLeaveEntitlement: not null, StartAt: not null })
                return false;

            var today = DateTime.Now;
            var startDate = employee.StartAt.Value.Date;

            // Calculate the years employed and the start of the current work year
            var yearsEmployed = (today - startDate).Days / 365.25;
            var currentYearStartDate = startDate.AddYears((int)yearsEmployed);

            // Check if we need to reset the balance at the start of the current work year
            if (employee.ResetAnnualLeave && today == currentYearStartDate)
            {
                employee.AnnualLeaveBalance = 0;
            }

            // Calculate daily accrual
            var annualEntitlement = employee.AnnualLeaveEntitlement.Value;
            var monthlyAccrual = annualEntitlement / 12.0;
            var daysInCurrentMonth = DateTime.DaysInMonth(today.Year, today.Month);
            var dailyAccrual = monthlyAccrual / daysInCurrentMonth;

            // Calculate the future balance up to the leave start date
            var daysUntilLeave = (today - leaveStartDate).Days;
            var futureAccrual = daysUntilLeave * dailyAccrual;
            var projectedBalance = employee.AnnualLeaveBalance + futureAccrual;

            // Round the projected balance to two decimal places
            projectedBalance = Math.Round(projectedBalance, 2);

            // Check if the projected balance is enough for the requested leave
            return projectedBalance >= requestedLeaveDays;
        }



        public async Task<IActionResult> DownloadDocument(string fileName)
        {
            var filePath = _webHostEnvironment.WebRootPath + fileName;

            if (!System.IO.File.Exists(filePath)) return NotFound();

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/octet-stream", Path.GetFileName(fileName));
        }
        
        public async Task<IActionResult> DeleteDocument(int? leaveRequestId)
        {
            if (leaveRequestId is null)
                return NotFound();

            if (leaveRequestId is 0)
            {
                return RedirectToAction(nameof(Upsert));
            }

            var leaveRequest = await _unitOfWork.LeaveRequest.Get(lr => lr.Id == leaveRequestId);
            if (leaveRequest == null)
                return NotFound();
            
            if (!string.IsNullOrEmpty(leaveRequest.Document))
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, leaveRequest.Document.TrimStart('\\'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
            else
            {
                return RedirectToAction(nameof(Upsert), new { id = leaveRequestId });
            }

            leaveRequest.Document = null;
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Upsert), new { id = leaveRequestId });
        }
        
        /*[HttpPost]*/
        public async Task<IActionResult> Delete(int id)
        {
            var leaveRequest = await _unitOfWork.LeaveRequest.Get(lr => lr.Id == id);
            var employeeId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = await _unitOfWork.ApplicationUser.Get(e => e.Id == employeeId);
            if (leaveRequest == null || employee is null)
                return NotFound();

            /*if (leaveRequest.LeaveStatus == "Approved")
            {
                var employee = await _unitOfWork.ApplicationUser.Get(e => e.Id == leaveRequest.EmployeeId);
                if (employee is not null)
                {
                    employee.AnnualLeaveBalance += leaveRequest.NumberOfDays;
                }
            }*/

            _unitOfWork.LeaveRequest.Remove(leaveRequest);
            await _unitOfWork.SaveAsync();
            TempData["success"] = "Leave request deleted successfully";
            return employee.IsManager 
                ? RedirectToAction(nameof(GetById), new { id = leaveRequest.EmployeeId })
                : RedirectToAction(nameof(Index));
        }
        
        /*
        public IActionResult Calculate()
        {
            var leaveBalance = new LeaveBalanceCalculatorVm();
            return View(leaveBalance);
        }
        
        [HttpPost]
        public async Task<IActionResult> Calculate(LeaveBalanceCalculatorVm leaveBalance)
        {
            if (!ModelState.IsValid)
                return View(leaveBalance);

            var employee = await _unitOfWork.ApplicationUser.Get(e => e.Id == leaveBalance.EmployeeId);
            if (employee is null)
            {
                ModelState.AddModelError("", "Employee not found");
                return View(leaveBalance);
            }

            var employeeLeaveBalance = await _unitOfWork.EmployeeLeaveBalance.Get(
                elb => elb.EmployeeId == leaveBalance.EmployeeId && elb.LeaveTypeId == leaveBalance.LeaveTypeId);

            if (employeeLeaveBalance is null)
            {
                ModelState.AddModelError("", "Employee leave balance not found");
                return View(leaveBalance);
            }

            var leaveType = await _unitOfWork.LeaveType.Get(lt => lt.Id == leaveBalance.LeaveTypeId);
            if (leaveType is null)
            {
                ModelState.AddModelError("", "Leave type not found");
                return View(leaveBalance);
            }
            
            // Calculate daily accrual
            var annualEntitlement = employee.AnnualLeaveEntitlement!.Value;
            var monthlyAccrual = annualEntitlement / 12.0;
            var daysInCurrentMonth = DateTime.DaysInMonth(today.Year, today.Month);
            var dailyAccrual = monthlyAccrual / daysInCurrentMonth;

            // Calculate the future balance up to the leave start date
            var daysUntilLeave = (today - leaveStartDate).Days;
            var futureAccrual = daysUntilLeave * dailyAccrual;
            var projectedBalance = employee.AnnualLeaveBalance + futureAccrual;

            // Round the projected balance to two decimal places
            projectedBalance = Math.Round(projectedBalance, 2);

            /*var monthsToAccrue = 1;
            var additionalAccrual = leaveType.MaxDaysPerYear / 12 * monthsToAccrue;
            var newBalance = employeeLeaveBalance.DaysUsed + additionalAccrual - leaveBalance.RequestedDays;
            leaveBalance.ProjectedBalance = newBalance;#1#

            return View(leaveBalance);
        }
        */
    

        private async Task FillSelectionData(LeaveRequestVm leaveRequestVm)
        {
            var employees = await _unitOfWork.ApplicationUser.GetAll();
            leaveRequestVm.Employees = employees.Select(e => new SelectListItem
            {
                Text = $"{e.FirstName} {e.LastName}",
                Value = e.Id
            }).ToList();
        
            var leaveTypes = await _unitOfWork.LeaveType.GetAll();
            leaveRequestVm.LeaveTypes = leaveTypes.Select(lt => new SelectListItem
            {
                Text = lt.Name,
                Value = lt.Id.ToString()
            }).ToList();
        }

        #region API CALLS

        public async Task<JsonResult> GetHolidays(DateTime? startDate, DateTime? endDate)
        {
            if (startDate is null || endDate is null)
            {
                var holidays = await _unitOfWork.Holiday.GetAll();
                return Json(holidays.Count());
            }
            var feltedHolidays = await _unitOfWork.Holiday
                .GetAll(h => h.StartAt >= startDate && h.EndAt <= endDate);
            return Json(feltedHolidays.Count());
        }

        #endregion
    }
}