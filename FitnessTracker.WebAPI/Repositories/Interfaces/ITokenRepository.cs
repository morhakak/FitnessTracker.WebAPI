using FitnessTracker.WebAPI.Models.Domain;

namespace FitnessTracker.WebAPI.Repositories.Interfaces;

public interface ITokenRepository
{
    Task<string> CreateJwtToken(User user);
}
