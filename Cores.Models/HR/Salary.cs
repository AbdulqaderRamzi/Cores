using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.HR;

public class Salary
{
    public int Id { get; set; }
    [Required]
    public decimal BaseSalary { get; set; }
    public decimal? Bonuses { get; set; }
    public decimal? Deductions { get; set; }
    [Required]
    public DateTime? EffectiveDate { get; set; }

    public string EmployeeId { get; set; }
    [ForeignKey("EmployeeId")]
    [ValidateNever]
    public ApplicationUser Employee { get; set; }
}