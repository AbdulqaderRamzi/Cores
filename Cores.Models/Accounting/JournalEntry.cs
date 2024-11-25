using System.Collections.ObjectModel;
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
    
    public string EntryNumber { get; set; }
    public DateTime EntryDate { get; set; }
    public string Description { get; set; }
    public bool IsPosted { get; set; }
    public DateTime? PostedDate { get; set; }
    public string PostedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }

    public List<JournalEntryDetail> Details { get; set; } = [];

}