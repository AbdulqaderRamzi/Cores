using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.HR;

public class JobApplication
{
    public int Id { get; set; }

    public int RecruitmentId { get; set; }
    [ForeignKey("RecruitmentId")]   
    [ValidateNever]
    public Recruitment Recruitment { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Phone { get; set; }
    [Required]
    public string Email { get; set; }
    
    public string? Resume { get; set; } // Upload Resume
    
    public DateTime DateTime { get; set; } = DateTime.Now;
        
    public string? Status { get; set; }

}