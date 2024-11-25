using Cores.Models.HR;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class LeaveRequestVm
{
    public LeaveRequest LeaveRequest { get; set; }
    [ValidateNever] 
    public List<SelectListItem> Employees { get; set; }
    [ValidateNever]
    public List<SelectListItem> LeaveTypes { get; set; }
    [ValidateNever]
    public string? EmployeeName { get; set; }
    
}