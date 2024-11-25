using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.HR;

public class Attendance
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly? TimeIn { get; set; }
    public TimeOnly? TimeOut { get; set; }
    public string? Status { get; set; }
    public bool IsPresent { get; set; } = true;
    public bool IsIgnored { get; set; }
    [Required]
    public string? EmployeeId { get; set; }
    [ForeignKey("EmployeeId")]
    [ValidateNever]
    public ApplicationUser Employee { get; set; }
}