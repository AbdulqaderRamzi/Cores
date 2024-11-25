using Cores.Models.HR;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class EmployeeDeductionVm
{
    public EmployeeDeduction EmployeeDeduction { get; set; }
    [ValidateNever]
    public List<SelectListItem> Employees { get; set; }
    [ValidateNever]
    public List<SelectListItem> Deductions { get; set; }
    public bool IsFromAttendance { get; set; }
    [ValidateNever]
    public int AttendanceId { get; set; }
}