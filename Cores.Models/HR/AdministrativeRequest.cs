using System.ComponentModel.DataAnnotations;

namespace Cores.Models.HR;

public class AdministrativeRequest : Request
{
    [Required]
    public string RequestType { get; set; }  // ID Card, Access Card, Parking Permit
    [Required]
    public string Purpose { get; set; }
    public DateTime RequiredDate { get; set; }
    public bool IsReplacement { get; set; }
    public string? ReplacementReason { get; set; }
    public string? AdditionalNotes { get; set; }
}