using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.WebAPI.Models.DTOs.Auth;

public class LoginUserDto
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
    public string Username { get; set; }
    [Required(ErrorMessage = "Password is required.")]
    [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[\\W_]).{6,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, one special character, and be at least 6 characters long.")]
    public string Password { get; set; }
}
