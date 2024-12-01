using System.ComponentModel.DataAnnotations;

namespace Cores.Models.HR;

public class BenefitsRequest : Request
{
    [Required]
    public string BenefitType { get; set; } // Health Insurance, Travel Allowance
    public DateTime EffectiveDate { get; set; }
    [Required]
    public string Details { get; set; }
    public string? SupportingDocuments { get; set; }
}
