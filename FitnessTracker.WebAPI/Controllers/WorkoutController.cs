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
public class WorkoutController : ControllerBase
{
    private readonly IWorkoutRepository _workoutRepository;
    private readonly ILogger<WorkoutController> _logger;

    public WorkoutController(IWorkoutRepository workoutRepository, ILogger<WorkoutController> logger)
    {
        _workoutRepository = workoutRepository;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "User,Admin")]
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
            Exercises = new List<Exercise>()
        };

        var createdWorkout = await _workoutRepository.CreateWorkoutAsync(workout);
        return CreatedAtAction(nameof(GetWorkoutById), new { id = createdWorkout.WorkoutId }, createdWorkout);
    }

    [HttpDelete("{workoudId}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> DeleteWorkout(Guid workoudId)
    {
        var workout = await _workoutRepository.GetWorkoutByIdAsync(workoudId);
        if (workout == null)
        {
            return NotFound();
        }

        await _workoutRepository.DeleteWorkoutAsync(workoudId);
        return NoContent();
    }

    [HttpPut("{workoutId}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> UpdateWorkout(Guid workoutId, [FromBody] UpdateWorkoutDto updateWorkoutDto)
    {
        _logger.LogInformation($"Received payload: {JsonSerializer.Serialize(updateWorkoutDto)}");

        var result = await _workoutRepository.UpdateWorkoutAsync(workoutId, updateWorkoutDto);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPut("{workoutId}/toggle-like")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> ToggleLikeWorkout(Guid workoutId)
    {
        var result = await _workoutRepository.ToggleLikeWorkoutAsync(workoutId);

        if (!result) return NotFound();

        return Ok();
    }

    [HttpPost("{workoutId}/add-exercise")]
    [Authorize(Roles = "User,Admin")]
    public async Task<ActionResult<Exercise>> AddExerciseToWorkout(Guid workoutId, [FromBody] CreateExerciseDto exerciseDto)
    {
        try
        {
            var exercise = new Exercise
            {
                ExerciseId = Guid.NewGuid(),
                Name = exerciseDto.Name,
                WorkoutId = workoutId
            };

            await _workoutRepository.AddExerciseToWorkoutAsync(workoutId, exercise);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding exercise to workout.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("exercise/{exerciseId}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> DeleteExercise(Guid exerciseId)
    {
        var result = await _workoutRepository.DeleteExerciseAsync(exerciseId);

        if (!result) return BadRequest();

        return Ok();
    }

    [HttpPut("exercise/{exerciseId}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> UpdateExercise(Guid exerciseId, [FromBody] UpdateExerciseDto updateExerciseDto)
    {
        _logger.LogInformation($"Received payload: {JsonSerializer.Serialize(updateExerciseDto)}");

        try
        {
            var result = await _workoutRepository.UpdateExerciseAsync(exerciseId, updateExerciseDto);

            if (!result)
            {
                return NotFound("Exercise not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating exercise");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("exercise/add-set")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> AddSet([FromBody] AddSetDto addSetDto)
    {
        var set = new Set
        {
            ExerciseId = addSetDto.ExerciseId,
            SetId = Guid.NewGuid(),
        };

       var result =  await _workoutRepository.AddSetToExerciseAsync(set, addSetDto);

        if(!result) return BadRequest();

        return Ok();
    }

    [HttpDelete("exercise/delete-set/{setId}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> DeleteSet(Guid setId)
    {
        var result = await _workoutRepository.DeleteSetAsync(setId);

        if (!result) return BadRequest();

        return Ok();
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<Workout>>> GetAllWorkouts()
    {
        var workouts = await _workoutRepository.GetAllWorkoutsAsync();

        return Ok(workouts);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<ActionResult<Workout>> GetWorkoutById(Guid id)
    {
        var workout = await _workoutRepository.GetWorkoutByIdAsync(id);

        if (workout == null) return NotFound();

        return Ok(workout);
    }

    [Authorize]
    [HttpGet("user")]
    [Authorize(Roles = "User,Admin")]
    public async Task<ActionResult<List<Workout>>> GetAllWorkoutsByUserId()
    {
        var userId = GetUserId();

        var workouts = await _workoutRepository.GetAllWorkoutsByUserIdAsync(userId);

        return Ok(workouts);
    }



    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
