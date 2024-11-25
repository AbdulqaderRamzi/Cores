using Cores.Models.HR;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class RecruitmentVm
{
    public Recruitment Recruitment { get; set; }
    [ValidateNever]
    public List<SelectListItem> Departments { get; set; }
}