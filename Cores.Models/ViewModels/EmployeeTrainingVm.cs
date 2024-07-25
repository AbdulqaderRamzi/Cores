using Cores.Models.HR;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class EmployeeTrainingVm
{
    public EmployeeTraining EmployeeTraining { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> Employees { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> Trainings { get; set; }
}