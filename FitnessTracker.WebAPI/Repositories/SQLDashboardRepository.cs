using FitnessTracker.WebAPI.Data;
using FitnessTracker.WebAPI.Models.Domain;
using FitnessTracker.WebAPI.Models.DTOs.Dashboard;
using FitnessTracker.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.WebAPI.Repositories;

public class SQLDashboardRepository : IDashboardRepository
{
    private readonly UserManager<User> _userManager;
    private readonly FitnessTrackerDbContext _dbContext;

    public SQLDashboardRepository(UserManager<User> userManager, FitnessTrackerDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);

        if (user == null)
        {
            return false;
        }

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            return false;
        }

        return true;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        return users;
    }

    public async Task<List<UserWithRolesDto>> GetAllUsersWithRolesAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var userWithRolesDtos = new List<UserWithRolesDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userWithRolesDtos.Add(new UserWithRolesDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles.ToList()
            });
        }

        return userWithRolesDtos;
    }
}
