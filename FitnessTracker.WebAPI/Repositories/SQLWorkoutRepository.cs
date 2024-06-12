using FitnessTracker.WebAPI.Data;
using FitnessTracker.WebAPI.Models.Domain;
using FitnessTracker.WebAPI.Models.DTOs.Workout;
using FitnessTracker.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FitnessTracker.WebAPI.Repositories;

public class SQLWorkoutRepository : IWorkoutRepository
{
    private readonly FitnessTrackerDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<SQLWorkoutRepository> _logger;

    public SQLWorkoutRepository(FitnessTrackerDbContext dbContext, UserManager<User> userManager,ILogger<SQLWorkoutRepository> logger)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Workout> CreateWorkoutAsync(Workout workout)
    {
        await _dbContext.Workouts.AddAsync(workout);
        await _dbContext.SaveChangesAsync();
        return workout;
    }

    public async Task<bool> DeleteWorkoutAsync(Guid id)
    {
        var workout = await _dbContext.Workouts.FindAsync(id);

        if (workout == null) return false;

        _dbContext.Workouts.Remove(workout);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleLikeWorkoutAsync(Guid workoutId)
    {
        var workout = await _dbContext.Workouts
            .Include(w => w.Exercises)
            .ThenInclude(e => e.Sets)
            .FirstOrDefaultAsync(w => w.WorkoutId == workoutId);

        if (workout == null) return false;

        workout.IsLiked = !workout.IsLiked;

        _dbContext.Workouts.Update(workout);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<Workout>?> GetAllWorkoutsAsync()
    {
        var workouts =  await _dbContext.Workouts.ToListAsync();

        return workouts;
    }

    public async Task<List<Workout>> GetAllWorkoutsByUserIdAsync(Guid userId)
    {
        return await _dbContext.Workouts
                             .Where(w => w.UserId == userId.ToString())
                             .Include(w => w.Exercises)
                             .ThenInclude(e => e.Sets)
                             .ToListAsync();
    }

    public async Task<Workout?> GetWorkoutByIdAsync(Guid id)
    {
        var workout =  await _dbContext.Workouts
                             .Include(w => w.Exercises)
                             .ThenInclude(e => e.Sets)
                             .FirstOrDefaultAsync(w => w.WorkoutId == id);

        return workout;
    }

    public async Task<bool> UpdateWorkoutAsync(Guid workoutId, UpdateWorkoutDto updateWorkoutDto)
    {
        var workout = await _dbContext.Workouts
            .Include(w => w.Exercises)
            .ThenInclude(e => e.Sets)
            .FirstOrDefaultAsync(w => w.WorkoutId == workoutId);

        if (workout == null)
        {
            return false;
        }

        workout.Name = updateWorkoutDto.Name;
        workout.IsLiked = updateWorkoutDto.IsLiked;
        workout.UpdatedAt = updateWorkoutDto.UpdatedAt;

        if (updateWorkoutDto.Exercises != null)
        {
            foreach (var updateExercise in updateWorkoutDto.Exercises)
            {
                Exercise exercise;
                if (updateExercise.ExerciseId.HasValue)
                {
                    exercise = workout.Exercises.FirstOrDefault(e => e.ExerciseId == updateExercise.ExerciseId.Value);
                    if (exercise == null)
                    {
                        continue;
                    }

                    exercise.Name = updateExercise.Name;
                }
                else
                {
                    exercise = new Exercise
                    {
                        ExerciseId = Guid.NewGuid(),
                        Name = updateExercise.Name,
                        WorkoutId = workout.WorkoutId
                    };
                    workout.Exercises.Add(exercise);
                }

                foreach (var updateSet in updateExercise.Sets)
                {
                    Set set;
                    if (updateSet.SetId.HasValue)
                    {
                        set = exercise.Sets.FirstOrDefault(s => s.SetId == updateSet.SetId.Value);
                        if (set == null)
                        {
                            continue;
                        }

                        set.Reps = updateSet.Reps;
                        set.Weight = updateSet.Weight;
                    }
                    else
                    {
                        set = new Set
                        {
                            SetId = Guid.NewGuid(),
                            Reps = updateSet.Reps,
                            Weight = updateSet.Weight,
                            ExerciseId = exercise.ExerciseId
                        };
                        exercise.Sets.Add(set);
                    }
                }
            }
        }

        _dbContext.Workouts.Update(workout);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task AddExerciseToWorkoutAsync(Guid workoutId, Exercise exercise)
    {
        try
        {
            var workout = await _dbContext.Workouts.FirstOrDefaultAsync(w => w.WorkoutId == workoutId);

            if (workout == null)
            {
                _logger.LogWarning("Workout with ID {WorkoutId} not found", workoutId);
                throw new InvalidOperationException($"Workout with ID {workoutId} not found");
            }

            workout.Exercises.Add(exercise);  // Assuming Workout entity has a collection of Exercises
            _dbContext.Exercises.Add(exercise);

            _dbContext.Entry(workout).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Exercise added to workout {WorkoutId}", workoutId);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency exception occurred while adding exercise to workout {WorkoutId}", workoutId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding exercise to workout {WorkoutId}", workoutId);
            throw;
        }
    }

    public async Task<bool> UpdateExerciseAsync(Guid exerciseId, UpdateExerciseDto updateExerciseDto)
    {
        _logger.LogInformation("Updating exercise with ID: {exerciseId}", exerciseId);

        try
        {
            var exercise = await _dbContext.Exercises.Include(e => e.Sets).FirstOrDefaultAsync(e => e.ExerciseId == exerciseId);

            if (exercise == null)
            {
                _logger.LogWarning("Exercise with ID: {exerciseId} not found", exerciseId);
                return false;
            }

            _logger.LogInformation("Exercise found: {exercise}", JsonSerializer.Serialize(exercise));

            exercise.Name = updateExerciseDto.Name;

            // Update sets
            foreach (var updatedSet in updateExerciseDto.Sets)
            {
                if (updatedSet.SetId.HasValue)
                {
                    var set = exercise.Sets.FirstOrDefault(s => s.SetId == updatedSet.SetId.Value);
                    if (set != null)
                    {
                        set.Reps = updatedSet.Reps;
                        set.Weight = updatedSet.Weight;
                    }
                    else
                    {
                        _logger.LogInformation("Adding new set with provided SetId: {setId}", updatedSet.SetId.Value);
                        var newSet = new Set
                        {
                            SetId = updatedSet.SetId.Value,
                            ExerciseId = exerciseId,
                            Reps = updatedSet.Reps,
                            Weight = updatedSet.Weight
                        };
                        exercise.Sets.Add(newSet);
                    }
                }
                else
                {
                    _logger.LogInformation("Adding new set with generated SetId");
                    var newSet = new Set
                    {
                        SetId = Guid.NewGuid(),
                        ExerciseId = exerciseId,
                        Reps = updatedSet.Reps,
                        Weight = updatedSet.Weight
                    };
                    exercise.Sets.Add(newSet);
                }
            }

            _dbContext.Exercises.Update(exercise);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Exercise updated successfully");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating exercise in repository");
            return false;
        }
    }

    public async Task<bool> AddSetToExerciseAsync(Set set, AddSetDto addSetDto)
    {
        // Fetch the workout along with its exercises in one go
        var workout = await _dbContext.Workouts
            .Include(w => w.Exercises)
            .ThenInclude(e => e.Sets)
            .FirstOrDefaultAsync(w => w.WorkoutId == addSetDto.WorkoutId);

        // Return false if the workout or the exercise is not found
        if (workout == null) return false;

        var exercise = workout.Exercises.FirstOrDefault(e => e.ExerciseId == addSetDto.ExerciseId);
        if (exercise == null) return false;

        // Add the set to the exercise
        exercise.Sets.Add(set);
        _dbContext.Sets.Add(set);

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteSetAsync(Guid setId)
    {
        // Find the set with the specified ID
        var set = await _dbContext.Sets
            .Include(s => s.Exercise) // Include the exercise to which this set belongs
            .ThenInclude(e => e.Workout) // Include the workout for the exercise
            .FirstOrDefaultAsync(s => s.SetId == setId);

        if (set == null) return false;

        set.Exercise.Sets.Remove(set);

        _dbContext.Sets.Remove(set);

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteExerciseAsync(Guid exerciseId)
    {
        try
        {
            // Find the exercise with the specified ID
            var exercise = await _dbContext.Exercises
                .Include(e => e.Sets) // Include the sets of this exercise
                .FirstOrDefaultAsync(e => e.ExerciseId == exerciseId);

            // Return false if the exercise is not found
            if (exercise == null) return false;

            // Remove all sets associated with this exercise
            _dbContext.Sets.RemoveRange(exercise.Sets);

            // Remove the exercise itself
            _dbContext.Exercises.Remove(exercise);

            // Save changes to the database
            await _dbContext.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting exercise with ID {ExerciseId}", exerciseId);
            throw;
        }
    }
}
