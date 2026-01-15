using System.ComponentModel.DataAnnotations;

namespace TRSB.Web.Models;
public class LoginViewModel
{
    [Required]
    public string UsernameOrEmail { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
}