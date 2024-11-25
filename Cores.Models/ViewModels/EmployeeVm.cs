using Cores.Models.HR;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class EmployeeVm
{
    [ValidateNever]
    public ApplicationUser Employee { get; set; }
    [ValidateNever]
    public List<SelectListItem> Roles { get; set; }
    [ValidateNever]
    public List<SelectListItem> Employees { get; set; }
    [ValidateNever]
    public List<SelectListItem> Departments { get; set; }
    [ValidateNever]
    public List<SelectListItem> Positions { get; set; }
    [ValidateNever]
    public List<CheckBox> LanguagesOptions { get; set; }
    [ValidateNever]
    public string Role { get; set; }
    [ValidateNever]
    public string SerializedEmployee { get; set; }
}   