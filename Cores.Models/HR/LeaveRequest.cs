using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.HR;

public class LeaveRequest
{
    public int Id { get; set; }
    public string LeaveStatus { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string Reason { get; set; }
    
    public string EmployeeID { get; set; }
    [ForeignKey("EmployeeID")]
    [ValidateNever]
    public ApplicationUser Employee { get; set; }

    public int LeaveTypeID { get; set; }
    [ForeignKey("LeaveTypeID")]
    [ValidateNever]
    public LeaveType LeaveType { get; set; }
}