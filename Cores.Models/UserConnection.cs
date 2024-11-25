using System.ComponentModel.DataAnnotations;

namespace Cores.Models;

public class UserConnection
{
    [Required]
    public string Username{ get; set; } 
    [Required]
    public string ChatRoom{ get; set; } 
}