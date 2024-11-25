using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.HR
{
    public class LeaveRequest
    {
        public int Id { get; set; }

        public string? LeaveStatus { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime EndDate { get; set; } = DateTime.Now;

        [Range(1, int.MaxValue, ErrorMessage = "Number of days must be greater than 0")]
        public int NumberOfDays { get; set; }

        [StringLength(500, ErrorMessage = "Reason cannot be longer than 500 characters")]
        public string Reason { get; set; }

        public string EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        [ValidateNever]
        public ApplicationUser Employee { get; set; }

        public string? ManagerId { get; set; }
        
        public bool? ManagerResponse { get; set; }
        public bool? HrResponse { get; set; }
        public bool IsPayed { get; set; } = true;
        public bool IsDeducted { get; set; }
        
        public DateTime? ManagerResponseDate { get; set; }
        public DateTime? HrResponseDate { get; set; }
        
        public string? ApprovedByHrId { get; set; }

        public int LeaveTypeId { get; set; }
        [ForeignKey("LeaveTypeId")]
        [ValidateNever]
        public LeaveType LeaveType { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.Now;

        public string? ResponseReason { get; set; }

        public string? Document { get; set; }
        

       
    }
}