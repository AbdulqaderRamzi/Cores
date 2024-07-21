using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.HR;

public class Training
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public string? Description { get; set; }
    [Required]
    public DateTime? TrainingDate { get; set; }
    public string TrainerId { get; set; }
    [ForeignKey("TrainerId")]
    [ValidateNever]
    public ApplicationUser Trainer { get; set; }

    public ICollection<EmployeeTraining> EmployeeTrainings { get; set; } = new Collection<EmployeeTraining>();
}