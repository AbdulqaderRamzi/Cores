using System.ComponentModel.DataAnnotations;

namespace Cores.Models.Accounting;

public class JournalType
{
     public int Id { get; set; }
     [Required] 
     public string Type { get; set; }
     public string? Description { get; set; }
}