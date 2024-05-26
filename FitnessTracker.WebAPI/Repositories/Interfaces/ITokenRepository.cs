using FitnessTracker.WebAPI.Models.Domain;

namespace FitnessTracker.WebAPI.Repositories.Interfaces;

public interface ITokenRepository
{
    string CreateJwtToken(User user);
}
