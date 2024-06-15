using FitnessTracker.WebAPI.Models.Domain;
using FitnessTracker.WebAPI.Models.DTOs.Dashboard;

namespace FitnessTracker.WebAPI.Repositories.Interfaces;

public interface IDashboardRepository
{
    Task<List<User>> GetAllUsersAsync();
    Task<bool> DeleteUserAsync(string userId);
    Task<List<UserWithRolesDto>> GetAllUsersWithRolesAsync();
}
