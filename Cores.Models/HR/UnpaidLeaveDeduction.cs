using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.HR;

public class UnpaidLeaveDeduction
{
    public int Id { get; set; }
    public decimal Deduction { get; set; }
    public DateTime DateTime { get; set; }
    public string ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    [ValidateNever] 
    public ApplicationUser ApplicationUser { get; set; }
}