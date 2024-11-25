using System.ComponentModel.DataAnnotations.Schema;
using Cores.Models.CRM;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cores.Models;

public class CheckBox
{
    public int Id{ get; set; }
    public string Value{ get; set; }
    public bool isChecked{ get; set; }
}