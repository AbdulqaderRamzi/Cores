using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cores.Models.HR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.VisualBasic;

namespace Cores.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    public string FirstName{ get; set; }
    
    public string? LastName{ get; set; }
    
    [Required] 
    public string Career { get; set; }
    
    [Required] 
    public string Gender { get; set; }
    
    [Required]
    public decimal Salary{ get; set; }
    
    [Required] 
    [DataType(DataType.Date)]  
    [Display(Name = "Date of Birth")]
    public DateOnly DateOfBirth { get; set; }
    
    [Display(Name = "Date Of Joining")]
    [DataType(DataType.Date)]  
    public DateOnly DateOfJoining { get; set; }
   
    [Display(Name = "Street Address")]
    public string? StreetAddress{ get; set; }
   
    public string? City{ get; set; }
    public string? State{ get; set; }
    public string? ImageUrl{ get; set; }
    
    public string? employmentStatus{ get; set; }
   
    public int? DepartmentId { get; set; } 
    
    [Required]
    public int? PositionID { get; set; }
    [ForeignKey("PositionID")]
    [ValidateNever]
    public Position? Position { get; set; }
    
    public string? ManagerID { get; set; }
    [ForeignKey("ManagerID")]
    [ValidateNever]
    public ApplicationUser Manager { get; set; }
   
    public ICollection<Language> Languages { get; set; } = new List<Language>();
    public ICollection<ApplicationUser> Subordinates { get; set; } = new List<ApplicationUser>();
    //public ICollection<Salary> Salaries { get; set; } = new Collection<Salary>();
    //public ICollection<Attendance> Attendances { get; set; } = new Collection<Attendance>();
    //public ICollection<LeaveRequest> LeaveRequests { get; set; } = new Collection<LeaveRequest>();
    
}