using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.HR;

public class Archive
{
    public int Id { get; set; }
    public string? EmployeeId { get; set; }
    [ForeignKey("EmployeeId")]
    [ValidateNever]
    public ApplicationUser Employee { get; set; }
    
    public int? ArchiveTypeId { get; set; }
    [ForeignKey("ArchiveTypeId")]
    [ValidateNever]
    public ArchiveType ArchiveType { get; set; }
    
    public string? Path { get; set; }
    public string? Description { get; set; }
    public DateOnly UploadDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public DateTime? ExpiryDate { get; set; }
}