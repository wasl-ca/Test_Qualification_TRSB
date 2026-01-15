using System.ComponentModel.DataAnnotations;

namespace TRSB.Web.Models;
public class RegisterViewModel
{

    [Required(ErrorMessage = "Le courriel est requis")]
    [EmailAddress(ErrorMessage = "Le format de courriel est invalide")]
    public string Email { get; set; } = null!;
    [Required(ErrorMessage = "Le nom d'usager est requis")]
     public string Username { get; set; } = null!;
    [Required(ErrorMessage = "Le nom est requis")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Le mot de passe est requis")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "La confirmation du mot de passe est requise")]
    [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas")]
    public string ConfirmPassword { get; set; } = null!;
}