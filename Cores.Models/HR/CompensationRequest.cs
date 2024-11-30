using System.ComponentModel.DataAnnotations;

namespace Cores.Models.HR;

public class CompensationRequest : Request
{
    [Required]
    public string RequestType { get; set; } // Salary Review, Overtime Claims
    [Required]
    public decimal RequestedAmount { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public string Justification { get; set; }
    public string? SupportingDocuments { get; set; }
}