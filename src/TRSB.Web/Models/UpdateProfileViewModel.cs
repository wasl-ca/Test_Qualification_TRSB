namespace TRSB.Web.Models;

public class UpdateProfileViewModel
{
    // Lecture seule
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Name { get; set; } = null!;
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
}