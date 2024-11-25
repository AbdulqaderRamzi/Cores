using System.ComponentModel.DataAnnotations;

namespace Cores.Models.CRM;

public class EventType
{
    public int Id { get; set; }
    [Required]
    public string Type { get; set; }
    public string? Description { get; set; }
}