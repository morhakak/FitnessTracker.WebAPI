namespace FitnessTracker.WebAPI.Models.DTOs.Auth;

public class LoginResponseDto
{
    public string UserId { get; internal set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = false;
    public bool IsSuccess { get; set; } = false;
    public string? ErrorMessage { get; set; } = string.Empty;
    public string? Token { get; set; } = string.Empty;
    public string? Message {  get; set; } = string.Empty;
}