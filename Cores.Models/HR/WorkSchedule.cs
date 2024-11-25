using System.ComponentModel.DataAnnotations;

namespace Cores.Models.HR;

public class WorkSchedule
{
    public int Id { get; set; }
    [Required]
    public DayOfWeek? DayOfWeek { get; set; }
    [Required]
    public TimeSpan? StartTime { get; set; } 
    [Required]
    public TimeSpan? EndTime { get; set; }
}