using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.Accounting;

public class JournalEntry
{
    public int Id { get; set; }
    
    public int JournalId { get; set; }
    [ForeignKey("JournalId")]
    [ValidateNever]
    public Journal Journal { get; set; }
    
    public int AccountId { get; set; }  
    [ForeignKey("AccountId")]
    [ValidateNever]
    public Account Account { get; set; }
    
    public string ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    [ValidateNever]
    public ApplicationUser ApplicationUser { get; set; }
    
    [Required]
    public decimal? Debit { get; set; }
    [Required]
    public decimal? Credit { get; set; }
    [Required]
    public decimal? Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}