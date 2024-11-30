using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.HR;


public abstract class Request
{
    public int Id { get; set; }
    
    public string ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    [ValidateNever]
    public ApplicationUser Employee { get; set; }
    
    public DateTime RequestDate { get; set; } = DateTime.Now;
    public string Status { get; set; } = "Pending";
    
    public string? ApprovedById { get; set; }
    [ForeignKey("ApprovedById")]
    public ApplicationUser? ApprovedBy { get; set; }
    
    public DateTime? ApprovalDate { get; set; }
    public string? Comments { get; set; }
}