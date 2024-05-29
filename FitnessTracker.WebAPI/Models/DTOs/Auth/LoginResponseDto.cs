namespace FitnessTracker.WebAPI.Models.DTOs.Auth;

public class LoginResponseDto
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; } = string.Empty;
    public string? Token { get; set; } = string.Empty;
}