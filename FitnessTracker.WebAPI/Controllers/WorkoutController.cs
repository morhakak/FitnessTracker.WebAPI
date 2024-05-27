using FitnessTracker.WebAPI.CustomActionFilters;
using FitnessTracker.WebAPI.Models.Domain;
using FitnessTracker.WebAPI.Models.DTOs.Workout;
using FitnessTracker.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessTracker.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WorkoutController : ControllerBase
{
    private readonly IWorkoutRepository _workoutRepository;

    public WorkoutController(IWorkoutRepository workoutRepository)
    {
        _workoutRepository = workoutRepository;
    }

    [HttpPost]
    [Authorize(Policy = "AppUser")]
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
            Date = DateTime.Now,
            Exercises = createWorkoutRequest.Exercises.Select(e => new Exercise
            {
                Name = e.Name,
                Sets = e.Sets.Select(s => new Set
                {
                    Reps = s.Reps,
                    Weight = s.Weight
                }).ToList()
            }).ToList()
        };

        var createdWorkout = await _workoutRepository.CreateWorkoutAsync(workout);
        return CreatedAtAction(nameof(GetWorkoutById), new { id = createdWorkout.WorkoutId }, createdWorkout);
    }

    [HttpGet]
    public async Task<ActionResult<List<Workout>>> GetAllWorkouts()
    {
        var workouts = await _workoutRepository.GetAllWorkoutsAsync();

        return Ok(workouts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Workout>> GetWorkoutById(Guid id)
    {
        var workout = await _workoutRepository.GetWorkoutByIdAsync(id);

        if (workout == null) return NotFound();

        return Ok(workout);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<Workout>>> GetAllWorkoutsByUserId()
    {
        var userId = GetUserId();

        var workouts = await _workoutRepository.GetAllWorkoutsAsync();

        return Ok(workouts);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWorkout(Guid id, Workout workout)
    {
        var userId = GetUserId().ToString();

        if (id != workout.WorkoutId || workout.UserId != userId)
        {
            return BadRequest();
        }

        await _workoutRepository.UpdateWorkoutAsync(workout);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWorkout(Guid id)
    {
        var userId = GetUserId().ToString();

        var workout = await _workoutRepository.GetWorkoutByIdAsync(id);
        if (workout == null || workout.UserId != userId)
        {
            return NotFound();
        }

        await _workoutRepository.DeleteWorkoutAsync(id);
        return NoContent();
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
