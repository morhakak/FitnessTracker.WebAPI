using FitnessTracker.WebAPI.Models.Domain;
using FitnessTracker.WebAPI.Models.DTOs.Auth;
using FitnessTracker.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FitnessTracker.WebAPI.Repositories
{
    public class SQLAuthRepository : IAuthRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenRepository _tokenRepository;

        public SQLAuthRepository(UserManager<User> userManager, IConfiguration configuration, ITokenRepository tokenRepository)
        {
            _userManager = userManager;
            _configuration = configuration;
            _tokenRepository = tokenRepository;
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

            var result = await _userManager.CreateAsync(user, registerUserDto.Password);
            return result;
        }

        public async Task<string?> LoginAsync(string username, string password)
        {
           var existingUser =  await _userManager.FindByNameAsync(username);

            if (existingUser == null || !await _userManager.CheckPasswordAsync(existingUser, password))
            {
                return null;
            }

            var token = _tokenRepository.CreateJwtToken(existingUser);

            return token;
        }

        private string GenerateJwtToken(User existingUser)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, existingUser.Id),
                new Claim(ClaimTypes.Name, existingUser.UserName!),
                new Claim(ClaimTypes.Email, existingUser.Email!),
                //todo: add role
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
