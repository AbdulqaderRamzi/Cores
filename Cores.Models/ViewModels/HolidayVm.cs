using Cores.Models.HR;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class HolidayVm
{
    public Holiday Holiday { get; set; }
    [ValidateNever]
    public IEnumerable<SelectListItem> HolidayTypes { get; set; }
    // Dropdown options for DayOfWeek
    public IEnumerable<SelectListItem> DaysOfWeek => Enum.GetValues(typeof(DayOfWeek))
        .Cast<DayOfWeek>()
        .Select(d => new SelectListItem
        {
            Value = ((int)d).ToString(),
            Text = d.ToString()
        });
}