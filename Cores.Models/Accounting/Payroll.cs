using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.Accounting;

public class Payroll
{
    public int Id { get; set; }
    [Required]
    public DateTime? PayPeriodStart { get; set; }
    [Required]
    public DateTime? PayPeriodEnd { get; set; }
    [Required]
    public decimal? GrossPay { get; set; }
    [Required]
    public decimal? TotalDeduction { get; set; }
    [Required]
    public decimal? TotalBenefit { get; set; }
    [Required]
    public decimal? NetPay { get; set; }
    [Required]
    public DateTime? PaymentDate { get; set; }

    public string EmployeeId { get; set; }
    [ForeignKey("EmployeeId")]
    [ValidateNever]
    public ApplicationUser Employee { get; set; }
}