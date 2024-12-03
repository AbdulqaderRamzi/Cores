using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.HR;

public class LeaveType
{
    public int Id { get; set; }
    public string Name { get; set; }
    [Display(Name = "Maximum Days Per Year")]
    public int? MaxDaysPerYear { get; set; }   
    public string? Description { get; set; }
    
}