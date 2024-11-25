using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Cores.Models.HR;

public class HolidayType
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }  // e.g., "National", "Regional", "Company", "Religious"

    [StringLength(200)]
    public string Description { get; set; }

    // For color coding in UI or other categorization
    [StringLength(7)]
    public string ColorCode { get; set; }

    [Required]
    public DateTime? CreatedAt { get; set; }

    public bool IsActive { get; set; } = true;
    
    // Navigation property for related holidays
    public ICollection<Holiday> Holidays { get; set; } = new Collection<Holiday>();
}