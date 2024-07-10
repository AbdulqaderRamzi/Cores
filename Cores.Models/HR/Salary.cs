using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.HR;

public class Salary
{
    public int Id { get; set; }

    public decimal BaseSalary { get; set; }
    public decimal Bonuses { get; set; }
    public decimal Deductions { get; set; }
    public DateOnly EffectiveDate { get; set; }

    public string EmployeeID { get; set; }
    [ForeignKey("EmployeeID")]
    [ValidateNever]
    public ApplicationUser Employee { get; set; }
}