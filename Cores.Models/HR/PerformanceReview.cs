using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.HR;

public class PerformanceReview
{
    public int Id { get; set; }

    public string EmployeeId { get; set; }
    [ForeignKey("EmployeeId")]
    [ValidateNever]
    public ApplicationUser Employee { get; set; }

    public string ReviewerId { get; set; }
    [ForeignKey("ReviewerId")]
    [ValidateNever]
    public ApplicationUser Reviewer { get; set; }

    public DateOnly ReviewDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    [Required]
    public int? PerformanceScore { get; set; }
    public string? Comments { get; set; }
}