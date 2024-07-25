using Cores.Models.HR;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class PerformanceReviewVm
{
    public PerformanceReview PerformanceReview { get; set; }
    [ValidateNever] 
    public List<SelectListItem> Employees { get; set; }
    /*public string FormattedReviewDate { get; set; }*/
}