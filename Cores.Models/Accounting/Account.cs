using Cores.Models.Accounting.Enums;

namespace Cores.Models.Accounting;

public class Account
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Code { get; set; }
    public string Type { get; set; }
    public string Category { get; set; }
    public decimal Balance { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
}