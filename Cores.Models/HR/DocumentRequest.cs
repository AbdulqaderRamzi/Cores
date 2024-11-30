using System.ComponentModel.DataAnnotations;

namespace Cores.Models.HR;

public class DocumentRequest : Request
{
    [Required]
    public string DocumentType { get; set; } // Employment Certificate, Salary Certificate, etc.
    public string Purpose { get; set; }
    public string? AdditionalDetails { get; set; }
}