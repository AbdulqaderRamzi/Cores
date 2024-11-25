using Cores.Models.HR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class JobApplicationVm
{
    public JobApplication JobApplication { get; set; }
    [ValidateNever]
    public List<SelectListItem> Recruitments { get; set; } 
}