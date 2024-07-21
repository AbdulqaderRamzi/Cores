using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.HR;

public class EmployeeTraining
{
    public int Id { get; set; }

    public string EmployeeId { get; set; }
    [ForeignKey("EmployeeId")]
    [ValidateNever]
    public ApplicationUser Employee { get; set; }

    public int TrainingId { get; set; }
    [ForeignKey("TrainingId")]
    [ValidateNever]
    public Training Training { get; set; }    
    
    [Required]
    public string CompletionStatus { get; set; } // Completed, In Progress
    [Required]
    public DateTime? CompletionDate { get; set; }
}