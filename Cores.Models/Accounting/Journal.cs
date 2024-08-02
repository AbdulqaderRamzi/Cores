using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.Accounting;

public class Journal
{
    public int Id { get; set; }
    public DateTime? Date { get; set; } = DateTime.Now;
    
    public int JournalTypeId { get; set; } 
    
    [ForeignKey("JournalTypeId")]
    [ValidateNever]
    public JournalType JournalType { get; set; }  // General, Sales, Purchase

    public string? Description { get; set; }
}