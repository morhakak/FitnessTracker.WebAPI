using FitnessTracker.WebAPI.Models.Domain;
using FitnessTracker.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracker.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashboardController(IDashboardRepository dashboardRepository) : ControllerBase
{
    [HttpGet("users")]
    public async Task<ActionResult<List<User>>> GetUsers()
    {
        var users = await dashboardRepository.GetAllUsersWithRolesAsync();

        if (users is null) return BadRequest();

        return Ok(users);
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser([FromRoute] string userId)
    {
        var response = await dashboardRepository.DeleteUserAsync(userId);

        if (!response) return BadRequest(response);

        return NoContent();
    }
}
