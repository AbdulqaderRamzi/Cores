using System.ComponentModel.DataAnnotations;

namespace Cores.Models.HR;

public class ArchiveType
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public string? Description { get; set; }
}