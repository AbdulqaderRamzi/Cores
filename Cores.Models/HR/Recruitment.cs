using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.HR;

public class Recruitment
{
    public int Id { get; set; }

    [Required]
    public string JobTitle { get; set; }
    
    public int DepartmentId { get; set; }
    [ForeignKey("DepartmentId")]
    [ValidateNever]
    public Department Department { get; set; }
    public int? NumberOfVacancies { get; set; }
    public string? JobDescription { get; set; }
    [Required]
    public DateTime? PostingDate { get; set; }
    [Required]
    public DateTime? ClosingDate { get; set; }

    public ICollection<JobApplication> JobApplications { get; set; } = new Collection<JobApplication>();
}