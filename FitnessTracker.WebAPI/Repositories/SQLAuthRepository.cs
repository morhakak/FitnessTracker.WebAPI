using FitnessTracker.WebAPI.Models.Domain;
using FitnessTracker.WebAPI.Models.DTOs.Auth;
using FitnessTracker.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FitnessTracker.WebAPI.Repositories
{
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
                FirstName = registerUserDto.FirstName,
                LastName = registerUserDto.LastName
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

        public async Task<LoginResponseDto> LoginAsync(string username, string password)
        {
            var existingUser = await _userManager.FindByNameAsync(username);

            if (existingUser == null)
            {
                _logger.LogWarning($"Login attempt failed: User '{username}' not found.");
                return new LoginResponseDto { IsSuccess = false, ErrorMessage = "Invalid username or password." };
            }

            var passwordValid = await _userManager.CheckPasswordAsync(existingUser, password);

            if (!passwordValid)
            {
                _logger.LogWarning($"Login attempt failed: Invalid username for user '{username}'.");
                return new LoginResponseDto { IsSuccess = false, ErrorMessage = "Invalid username or password." };
            }

            var token = _tokenRepository.CreateJwtToken(existingUser);

            _logger.LogInformation("User '{Username}' successfully logged in.", username);

            return new LoginResponseDto { IsSuccess = true, Token = token };
        }
    }
}
