    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

    namespace Cores.Models.HR;

    public class EmployeeBenefit
    {
        public int Id { get; set; }

        public string EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        [ValidateNever]
        public ApplicationUser Employee { get; set; }

        public int BenefitId { get; set; }
        [ForeignKey("BenefitId")]
        [ValidateNever]
        public Benefit Benefit { get; set; }
            
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } // Active, Expired
    }