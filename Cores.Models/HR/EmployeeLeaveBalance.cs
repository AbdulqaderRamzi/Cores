using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.HR;

public class EmployeeLeaveBalance
{
    public int Id { get; set; }
    
    public string EmployeeId { get; set; }
    [ForeignKey("EmployeeId")]
    [ValidateNever]
    public ApplicationUser Employee { get; set; }
    
    public int LeaveTypeId { get; set; }
    [ForeignKey("LeaveTypeId")]
    [ValidateNever]
    public LeaveType LeaveType { get; set; }
    
    public DateTime EndDate { get; set; }
    public DateTime CurrentDate { get; set; }

    public int DaysUsed { get; set; }
}