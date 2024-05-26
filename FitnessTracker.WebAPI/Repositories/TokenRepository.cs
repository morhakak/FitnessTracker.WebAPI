using FitnessTracker.WebAPI.Models.Domain;
using FitnessTracker.WebAPI.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FitnessTracker.WebAPI.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration _configuration;

        public TokenRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string CreateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
               new(ClaimTypes.NameIdentifier,user.Id),
               new(ClaimTypes.Email, user.Email!),
            };

            //Todo:
            //foreach (var role in user.Roles)
            //{
            //    claims.Add(new(ClaimTypes.Role, role));
            //}

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Jwt:Issuer"], 
                _configuration["Jwt:Audience"], 
                claims, 
                expires: DateTime.Now.AddDays(30),
                signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
        }
    }
}
