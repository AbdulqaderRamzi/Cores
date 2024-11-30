using System.Security.Principal;
using Cores.Models.HR;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cores.Models.ViewModels;

public class DocumentRequestVm
{
    public DocumentRequest DocumentRequest { get; set; }
    [ValidateNever]
    public List<SelectListItem> DocumentTypes { get; set; }
}