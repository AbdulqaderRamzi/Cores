using System.Collections.ObjectModel;
using Cores.Models.Accounting.Enums;

namespace Cores.Models.Accounting;

public class Journal
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public bool IsActive { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }

    public ICollection<JournalEntry> Entries { get; set; } = new Collection<JournalEntry>();
}