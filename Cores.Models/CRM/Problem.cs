using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.CRM;

public class Problem
{
    public int Id { get; set; }
    
    public int ContactId { get; set; }
    [ForeignKey("ContactId")]
    [ValidateNever]
    public Contact Contact { get; set; }
    
    public DateTime ReportedDate { get; set; } = DateTime.Now;
    
    public int ProblemTypeId { get; set; }
    [ForeignKey("ProblemTypeId")]
    [ValidateNever]
    public ProblemType ProblemType { get; set; }
    [Required]
    public string Severity { get; set; }
    [Required]
    public string Status { get; set; }  
    public string? Description { get; set; }
    
    public string? ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    [ValidateNever]
    public ApplicationUser ApplicationUser { get; set; }
    
    public string? ModifiedById  { get; set; }
    [ForeignKey("ModifiedById")]
    [ValidateNever]
    public ApplicationUser ModifiedBy { get; set; }
    
    public DateTime? DateResolved { get; set; }
    public string? Resolution { get; set; }
    public string? Note { get; set; }
    
}