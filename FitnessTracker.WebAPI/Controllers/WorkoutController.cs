using FitnessTracker.WebAPI.CustomActionFilters;
using FitnessTracker.WebAPI.Models.Domain;
using FitnessTracker.WebAPI.Models.DTOs.Workout;
using FitnessTracker.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace FitnessTracker.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WorkoutController(IWorkoutRepository workoutRepository, ILogger<WorkoutController> logger) : ControllerBase
{
    private readonly ILogger<WorkoutController> _logger = logger;

    [HttpPost]
    [Authorize(Roles = "user,admin")]
    [ValidateModel]
    public async Task<ActionResult<Workout>> CreateWorkout([FromBody] CreateWorkoutRequest createWorkoutRequest)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null) return NotFound("User not found");

        var workout = new Workout
        {
            WorkoutId = Guid.NewGuid(),
            UserId = userId!,
            Name = createWorkoutRequest.Name,
            CreatedAt = DateTime.Now.ToString(),
            IsLiked = false,
            Exercises = []
        };

        var createdWorkout = await workoutRepository.CreateWorkoutAsync(workout);
        return CreatedAtAction(nameof(GetWorkoutById), new { id = createdWorkout.WorkoutId }, createdWorkout);
    }

    [HttpDelete("{workoudId}")]
    [Authorize(Roles = "user,admin")]
    public async Task<IActionResult> DeleteWorkout(Guid workoudId)
    {
        var workout = await workoutRepository.GetWorkoutByIdAsync(workoudId);
        if (workout == null)
        {
            return NotFound();
        }

        await workoutRepository.DeleteWorkoutAsync(workoudId);
        return NoContent();
    }

    [HttpPut("{workoutId}")]
    [Authorize(Roles = "user,admin")]
    public async Task<IActionResult> UpdateWorkout(Guid workoutId, [FromBody] UpdateWorkoutDto updateWorkoutDto)
    {
        _logger.LogInformation($"Received payload: {JsonSerializer.Serialize(updateWorkoutDto)}");

        var result = await workoutRepository.UpdateWorkoutAsync(workoutId, updateWorkoutDto);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPut("{workoutId}/toggle-like")]
    [Authorize(Roles = "user,admin")]
    public async Task<IActionResult> ToggleLikeWorkout(Guid workoutId)
    {
        var result = await workoutRepository.ToggleLikeWorkoutAsync(workoutId);

        if (!result) return NotFound();

        return Ok();
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<List<Workout>>> GetAllWorkouts()
    {
        var workouts = await workoutRepository.GetAllWorkoutsAsync();

        return Ok(workouts);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "user,admin")]
    public async Task<ActionResult<Workout>> GetWorkoutById(Guid id)
    {
        var workout = await workoutRepository.GetWorkoutByIdAsync(id);

        if (workout == null) return NotFound();

        return Ok(workout);
    }

    [Authorize]
    [HttpGet("user")]
    [Authorize(Roles = "user,admin")]
    public async Task<ActionResult<List<Workout>>> GetAllWorkoutsByUserId()
    {
        var userId = GetUserId();

        _logger.LogInformation($"User ID: {userId}");
        _logger.LogInformation($"User Claims: {string.Join(", ", User.Claims.Select(c => $"{c.Type}: {c.Value}"))}");

        var workouts = await workoutRepository.GetAllWorkoutsByUserIdAsync(userId);

        return Ok(workouts);
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
