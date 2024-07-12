using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.HR;

public class LeaveRequest
{
    public int Id { get; set; }
    public string? LeaveStatus { get; set; }
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public string? Reason { get; set; }
    
    public string EmployeeId { get; set; }
    [ForeignKey("EmployeeId")]
    [ValidateNever]
    public ApplicationUser Employee { get; set; }

    public int LeaveTypeId { get; set; }
    [ForeignKey("LeaveTypeId")]
    [ValidateNever]
    public LeaveType LeaveType { get; set; }
}