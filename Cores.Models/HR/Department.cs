using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models.HR;

public class Department
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string? DepartmentHeadId { get; set; }
    [ForeignKey("DepartmentHeadId")]
    [ValidateNever]
    public ApplicationUser DepartmentHead { get; set; }

    public string? Location { get; set; }

                /*   Because Of Circular dependency, this should be removed  */
    public ICollection<ApplicationUser> Employees { get; set; } = new Collection<ApplicationUser>();

}