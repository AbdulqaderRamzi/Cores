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
    public string Gender { get; set; }
    
    [Required]
    public decimal Salary{ get; set; }
    
    [Required] 
    [Display(Name = "Date of Birth")]
    public DateTime? DateOfBirth { get; set; }
     
    [Display(Name = "Date Of Joining")]
    public DateTime DateOfJoining { get; set; }
    
    [Display(Name = "Street Address")]
    public string? StreetAddress{ get; set; }
    
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? IPAN { get; set; }
    public string? CivilIdNumber { get; set; }
    public string? PassportNumber { get; set; }
    public DateTime? PassportExpiredDate { get; set; }
    public DateTime? ResidenceExpiredDate { get; set; }
    public DateTime? HealthCardExpiredDate { get; set; }
    public DateTime? DrivingLicenseExpiredDate { get; set; }
    public DateTime? StartAt { get; set; } = DateTime.Now;
    public string? EmergencyNumber { get; set; }

    public bool IsManager { get; set; }
    
    public string? City{ get; set; }
    public string? State{ get; set; }
    public string? ImageUrl{ get; set; }
    
    public double AnnualLeaveBalance { get; set; }
    public int? AnnualLeaveEntitlement { get; set; }
    public bool ResetAnnualLeave { get; set; }

    public string? Role { get; set; }
    
    public string? employmentStatus{ get; set; }
   
    public int? DepartmentId { get; set; } 
    [NotMapped]
    public string? DepartmentName { get; set; }
    [NotMapped]
    public string? Document { get; set; }
    
    [Required]
    [Range(0, byte.MaxValue)]
    public byte? WorkingDaysInMonth { get; set; }
    
    [Required]
    public int? PositionID { get; set; }
    [ForeignKey("PositionID")]
    [ValidateNever]
    public Position? Position { get; set; }
    /*
    [NotMapped]
    public string? PositionName { get; set; }
    */
    
    public string? ManagerID { get; set; }
    [ForeignKey("ManagerID")]
    [ValidateNever]
    public ApplicationUser Manager { get; set; }
    
    //public bool MustChangePassword { get; set; } = true; // Default to true for new users
    
    public ICollection<Language> Languages { get; set; } = new List<Language>();
    public ICollection<ApplicationUser> Subordinates { get; set; } = new List<ApplicationUser>();
    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new Collection<LeaveRequest>();
    public ICollection<Attendance> Attendances { get; set; } = new Collection<Attendance>();
    public ICollection<Salary> Salaries { get; set; } = new Collection<Salary>();
    //public ICollection<PerformanceReview> PerformanceReviews { get; set; } = new Collection<PerformanceReview>();
    public ICollection<EmployeeTraining> EmployeeTrainings { get; set; } = new Collection<EmployeeTraining>();
    public ICollection<EmployeeBenefit> EmployeeBenefits { get; set; } = new Collection<EmployeeBenefit>();
    public ICollection<EmployeeDeduction> EmployeeDeductions { get; set; } = new Collection<EmployeeDeduction>();
    public ICollection<Archive> Documents { get; set; } = new Collection<Archive>(); 
    public ICollection<WorkSchedule> WorkSchedules { get; set; } = new Collection<WorkSchedule>();
    public ICollection<EmployeeLeaveBalance> LeaveBalances { get; set; } = new Collection<EmployeeLeaveBalance>(); 
}   