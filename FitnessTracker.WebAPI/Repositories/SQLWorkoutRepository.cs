using FitnessTracker.WebAPI.Data;
using FitnessTracker.WebAPI.Models.Domain;
using FitnessTracker.WebAPI.Models.DTOs.Workout;
using FitnessTracker.WebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.WebAPI.Repositories;

public class SQLWorkoutRepository(FitnessTrackerDbContext dbContext, ILogger<SQLWorkoutRepository> logger) : IWorkoutRepository
{
    private readonly ILogger<SQLWorkoutRepository> _logger = logger;

    public async Task<Workout> CreateWorkoutAsync(Workout workout)
    {
        await dbContext.Workouts.AddAsync(workout);
        await dbContext.SaveChangesAsync();
        return workout;
    }

    public async Task<bool> DeleteWorkoutAsync(Guid id)
    {
        var workout = await dbContext.Workouts.FindAsync(id);

        if (workout == null) return false;

        dbContext.Workouts.Remove(workout);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleLikeWorkoutAsync(Guid workoutId)
    {
        var workout = await dbContext.Workouts
            .Include(w => w.Exercises)
            .ThenInclude(e => e.Sets)
            .FirstOrDefaultAsync(w => w.WorkoutId == workoutId);

        if (workout == null) return false;

        workout.IsLiked = !workout.IsLiked;

        dbContext.Workouts.Update(workout);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<Workout>?> GetAllWorkoutsAsync()
    {
        var workouts =  await dbContext.Workouts
                               .Include(w => w.Exercises.OrderBy(e => e.CreatedAt))
                               .ThenInclude(e => e.Sets.OrderBy(s => s.CreatedAt))
                               .OrderBy(w => w.CreatedAt)
                               .ToListAsync();
        return workouts;
    }

    public async Task<List<Workout>> GetAllWorkoutsByUserIdAsync(Guid userId)
    {
        var workouts = await dbContext.Workouts
                              .Where(w => w.UserId == userId.ToString())
                              .Include(w => w.Exercises.OrderBy(e => e.CreatedAt)) 
                              .ThenInclude(e => e.Sets.OrderBy(s => s.CreatedAt)) 
                              .OrderBy(w => w.CreatedAt) 
                              .ToListAsync();
        return workouts;
    }

    public async Task<Workout?> GetWorkoutByIdAsync(Guid id)
    {
        var workout =  await dbContext.Workouts
                              .Where(w => w.WorkoutId == id)
                              .Include(w => w.Exercises.OrderBy(e => e.CreatedAt))
                              .ThenInclude(e => e.Sets.OrderBy(s => s.CreatedAt))
                              .FirstOrDefaultAsync();
        return workout;
    }

    public async Task<bool> UpdateWorkoutAsync(Guid workoutId, UpdateWorkoutDto updateWorkoutDto)
    {
        using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            var workout = await dbContext.Workouts
                .Include(w => w.Exercises)
                .ThenInclude(e => e.Sets)
                .FirstOrDefaultAsync(w => w.WorkoutId == workoutId);

            if (workout == null)
            {
                _logger.LogWarning($"Workout with ID {workoutId} not found.");
                return false;
            }

            _logger.LogInformation($"Updating workout: {workoutId}");

            UpdateWorkoutProperties(workout, updateWorkoutDto);

            if (updateWorkoutDto.Exercises != null)
            {
                var processedExerciseIds = new HashSet<Guid>();

                foreach (var updateExercise in updateWorkoutDto.Exercises)
                {
                    var exercise = HandleExercise(workout, updateExercise);
                    processedExerciseIds.Add(exercise.ExerciseId);

                    HandleSets(exercise, updateExercise.Sets);
                }

                DeleteUnprocessedExercises(workout, processedExerciseIds);
            }

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation($"Workout {workoutId} updated successfully.");
            return true;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError($"Concurrency exception for workout {workoutId}: {ex.Message}");
            throw new Exception("Concurrency exception occurred.", ex);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError($"Error updating workout {workoutId}: {ex.Message}");
            throw new Exception("An error occurred while updating the workout.", ex);
        }
    }

    private void UpdateWorkoutProperties(Workout workout, UpdateWorkoutDto updateWorkoutDto)
    {
        workout.Name = updateWorkoutDto.Name;
        workout.IsLiked = updateWorkoutDto.IsLiked;
        workout.UpdatedAt = updateWorkoutDto.UpdatedAt;
    }

    private Exercise HandleExercise(Workout workout, UpdateExerciseDto updateExercise)
    {
        Exercise exercise = null;

        if (!string.IsNullOrEmpty(updateExercise.ExerciseId) && !updateExercise.ExerciseId.ToLower().Contains("temp"))
        {
            exercise = workout.Exercises.FirstOrDefault(e => e.ExerciseId.ToString() == updateExercise.ExerciseId);
        }

        if (exercise == null)
        {
            exercise = new Exercise
            {
                ExerciseId = Guid.NewGuid(),
                Name = updateExercise.Name,
                WorkoutId = workout.WorkoutId,
                CreatedAt = DateTime.Now.ToString(),
                Sets = []
            };
            workout.Exercises.Add(exercise);
            dbContext.Exercises.Add(exercise);
            _logger.LogInformation($"Added new exercise: {exercise.ExerciseId}");
        }
        else
        {
            exercise.Name = updateExercise.Name;
        }

        return exercise;
    }

    private void HandleSets(Exercise exercise, List<UpdateSetDto> updateSets)
    {
        var processedSetIds = new HashSet<Guid>();

        foreach (var updateSet in updateSets)
        {
            Set set = null;

            if (!string.IsNullOrEmpty(updateSet.SetId) && !updateSet.SetId.ToLower().Contains("temp"))
            {
                set = exercise.Sets.FirstOrDefault(s => s.SetId.ToString() == updateSet.SetId);
            }

            if (set == null)
            {
                set = new Set
                {
                    SetId = Guid.NewGuid(),
                    Reps = updateSet.Reps,
                    Weight = updateSet.Weight,
                    ExerciseId = exercise.ExerciseId,
                    CreatedAt = DateTime.Now.ToString()
                };
                exercise.Sets.Add(set);
                dbContext.Sets.Add(set);
                _logger.LogInformation($"Added new set: {set.SetId}");
            }
            else
            {
                set.Reps = updateSet.Reps;
                set.Weight = updateSet.Weight;
            }

            processedSetIds.Add(set.SetId);
        }

        DeleteUnprocessedSets(exercise, processedSetIds);
    }

    private void DeleteUnprocessedSets(Exercise exercise, HashSet<Guid> processedSetIds)
    {
        var setsToRemove = exercise.Sets.Where(s => !processedSetIds.Contains(s.SetId)).ToList();
        foreach (var setToRemove in setsToRemove)
        {
            exercise.Sets.Remove(setToRemove);
            dbContext.Sets.Remove(setToRemove);
            _logger.LogInformation($"Removed set: {setToRemove.SetId}");
        }
    }

    private void DeleteUnprocessedExercises(Workout workout, HashSet<Guid> processedExerciseIds)
    {
        var exercisesToRemove = workout.Exercises.Where(e => !processedExerciseIds.Contains(e.ExerciseId)).ToList();
        foreach (var exerciseToRemove in exercisesToRemove)
        {
            workout.Exercises.Remove(exerciseToRemove);
            dbContext.Exercises.Remove(exerciseToRemove);
            _logger.LogInformation($"Removed exercise: {exerciseToRemove.ExerciseId}");
        }
    }
}
