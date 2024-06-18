using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models;

public class CheckBox
{
    public int Id{ get; set; }
    public string Value{ get; set; }
    public bool isChecked{ get; set; }
    public string? EmployeeId{ get; set; }
    [ForeignKey("EmployeeId")]
    [ValidateNever]
    public ApplicationUser ApplicationUser{ get; set; }
}