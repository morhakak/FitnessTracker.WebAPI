using FitnessTracker.WebAPI.Models.Domain;
using FitnessTracker.WebAPI.Models.DTOs.Auth;
using FitnessTracker.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FitnessTracker.WebAPI.Repositories;

public class SQLAuthRepository : IAuthRepository
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ITokenRepository _tokenRepository;
    private readonly ILogger<SQLAuthRepository> _logger;

    public SQLAuthRepository(UserManager<User> userManager, IConfiguration configuration, ITokenRepository tokenRepository, ILogger<SQLAuthRepository> logger)
    {
        _userManager = userManager;
        _configuration = configuration;
        _tokenRepository = tokenRepository;
        _logger = logger;   
    }

    public async Task<IdentityResult> RegisterUserAsync(RegisterUserDto registerUserDto)
    {
        var user = new User
        {
            UserName = registerUserDto.Username,
            Email = registerUserDto.Email,
            CreatedAt = DateTime.Now.ToString()
        };

        var identityResult = await _userManager.CreateAsync(user, registerUserDto.Password);

        if (!identityResult.Succeeded) 
        {
            _logger.LogError("User creation failed: {Errors}", string.Join(", ", identityResult.Errors.Select(e => e.Description)));
         
            return identityResult;
        }

        var roleResult = await _userManager.AddToRoleAsync(user, "User");

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            _logger.LogError("Adding user to role failed: {Errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            return roleResult;
        }

        return IdentityResult.Success;
    }

    public async Task<LoginResponseDto> LoginAsync(User user,string password)
    {
        var passwordValid = await _userManager.CheckPasswordAsync(user, password);

        if (!passwordValid)
        {
            _logger.LogWarning($"Login attempt failed: Invalid username for user '{user.UserName}'.");
            return new LoginResponseDto { IsSuccess = false, ErrorMessage = "Invalid username or password." };
        }

        var token = _tokenRepository.CreateJwtToken(user);

        string message = $"User '{user.UserName}' successfully logged in";
        _logger.LogInformation(message);

        var roles = await _userManager.GetRolesAsync(user);

        var isAdmin = roles.Any(s => s.ToLower().Contains("admin"));

        return new LoginResponseDto { IsSuccess = true, UserId=user.Id, Username=user.UserName!,Email=user.Email!, IsAdmin=isAdmin, Token = token, Message=message };
    }
}
