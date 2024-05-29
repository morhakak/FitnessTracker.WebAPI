using FitnessTracker.WebAPI.Models.DTOs.Auth;
using Microsoft.AspNetCore.Identity;

namespace FitnessTracker.WebAPI.Repositories.Interfaces;

public interface IAuthRepository
{
    Task<IdentityResult> RegisterUserAsync(RegisterUserDto registerUserDto);
    Task<LoginResponseDto> LoginAsync(string username, string password);
}
