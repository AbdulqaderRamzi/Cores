using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.HR;

public class Holiday
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    public int HolidayTypeId { get; set; }
    [ForeignKey("HolidayTypeId")]
    [ValidateNever]
    public HolidayType HolidayType { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    public bool IsRecurringYearly { get; set; }

    [Required]
    public bool IsWorkingDay { get; set; }

    [StringLength(50)]
    public string Location { get; set; }
    
    public DayOfWeek? DayOfWeek { get; set; }

    [Required] 
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public DateTime? StartAt { get; set; }
    
    public DateTime? EndAt { get; set; }
    
    public bool IsActive { get; set; } = true;
}