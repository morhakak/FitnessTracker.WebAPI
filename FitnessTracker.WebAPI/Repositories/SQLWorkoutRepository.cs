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
        var workouts =  await _dbContext.Workouts
                               .Include(w => w.Exercises.OrderBy(e => e.CreatedAt))
                               .ThenInclude(e => e.Sets.OrderBy(s => s.CreatedAt))
                               .OrderBy(w => w.CreatedAt)
                               .ToListAsync();
        return workouts;
    }

    public async Task<List<Workout>> GetAllWorkoutsByUserIdAsync(Guid userId)
    {
        var workouts = await _dbContext.Workouts
                              .Where(w => w.UserId == userId.ToString())
                              .Include(w => w.Exercises.OrderBy(e => e.CreatedAt)) 
                              .ThenInclude(e => e.Sets.OrderBy(s => s.CreatedAt)) 
                              .OrderBy(w => w.CreatedAt) 
                              .ToListAsync();
        return workouts;
    }

    public async Task<Workout?> GetWorkoutByIdAsync(Guid id)
    {
        var workout =  await _dbContext.Workouts
                              .Where(w => w.WorkoutId == id)
                              .Include(w => w.Exercises.OrderBy(e => e.CreatedAt))
                              .ThenInclude(e => e.Sets.OrderBy(s => s.CreatedAt))
                              .FirstOrDefaultAsync();
        return workout;
    }

    //public async Task<bool> UpdateWorkoutAsync(Guid workoutId, UpdateWorkoutDto updateWorkoutDto)
    //{

    //    var workout = await _dbContext.Workouts
    //        .Include(w => w.Exercises)
    //        .ThenInclude(e => e.Sets)
    //        .FirstOrDefaultAsync(w => w.WorkoutId == workoutId);

    //    if (workout == null)
    //    {
    //        return false;
    //    }

    //    // Update properties
    //    workout.Name = updateWorkoutDto.Name;
    //    workout.IsLiked = updateWorkoutDto.IsLiked;
    //    workout.UpdatedAt = updateWorkoutDto.UpdatedAt;

    //    if (updateWorkoutDto.Exercises != null)
    //    {
    //        foreach (var updateExercise in updateWorkoutDto.Exercises)
    //        {
    //            Exercise exercise = null;
    //            if (!string.IsNullOrEmpty(updateExercise.ExerciseId) && !updateExercise.ExerciseId.ToLower().Contains("temp"))// if exercise id !empty and !cotains the word temp
    //            {
    //                exercise = workout.Exercises.FirstOrDefault(e => e.ExerciseId.ToString() == updateExercise.ExerciseId);
    //                if (exercise == null)
    //                {
    //                    continue;
    //                }

    //                exercise.Name = updateExercise.Name;
    //            }
    //            else if(string.IsNullOrEmpty(updateExercise.ExerciseId) || updateExercise.ExerciseId.ToLower().Contains("temp"))
    //            {
    //                exercise = new Exercise
    //                {
    //                    ExerciseId = Guid.NewGuid(),
    //                    Name = updateExercise.Name,
    //                    WorkoutId = workout.WorkoutId,
    //                    CreatedAt = DateTime.Now.ToString()
    //                };
    //                workout.Exercises.Add(exercise);
    //                _dbContext.Exercises.Add(exercise);
    //            }

    //            foreach (var updateSet in updateExercise.Sets)
    //            {
    //                Set set = null;
    //                if (!string.IsNullOrEmpty(updateSet.SetId) && !updateSet.SetId.ToLower().Contains("temp"))
    //                {
    //                    set = exercise.Sets.FirstOrDefault(s => s.SetId.ToString() == updateSet.SetId);
    //                    if (set == null)
    //                    {
    //                        continue;
    //                    }

    //                    set.Reps = updateSet.Reps;
    //                    set.Weight = updateSet.Weight;
    //                }
    //                else if(string.IsNullOrEmpty(updateSet.SetId) || updateSet.SetId.ToLower().Contains("temp"))
    //                {
    //                    set = new Set
    //                    {
    //                        SetId = Guid.NewGuid(),
    //                        Reps = updateSet.Reps,
    //                        Weight = updateSet.Weight,
    //                        ExerciseId = exercise.ExerciseId,
    //                        CreatedAt = DateTime.Now.ToString()
    //                    };
    //                    exercise.Sets.Add(set);
    //                    _dbContext.Sets.Add(set);
    //                }
    //            }
    //        }
    //    }

    //    // Attach the original RowVersion value
    //    //_dbContext.Entry(workout).Property(w => w.RowVersion).OriginalValue = updateWorkoutDto.RowVersion;

    //    try
    //    {
    //        await _dbContext.SaveChangesAsync();
    //    }
    //    catch (DbUpdateConcurrencyException ex)
    //    {
    //        // Handle the concurrency exception as needed
    //        throw new Exception("Concurrency exception occurred.", ex);
    //    }

    //    return true;
    //}

    public async Task<bool> UpdateWorkoutAsync(Guid workoutId, UpdateWorkoutDto updateWorkoutDto)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var workout = await _dbContext.Workouts
                .Include(w => w.Exercises)
                .ThenInclude(e => e.Sets)
                .FirstOrDefaultAsync(w => w.WorkoutId == workoutId);

            if (workout == null)
            {
                _logger.LogWarning($"Workout with ID {workoutId} not found.");
                return false;
            }

            _logger.LogInformation($"Updating workout: {workoutId}");

            // Update workout properties
            workout.Name = updateWorkoutDto.Name;
            workout.IsLiked = updateWorkoutDto.IsLiked;
            workout.UpdatedAt = updateWorkoutDto.UpdatedAt;

            if (updateWorkoutDto.Exercises != null)
            {
                var processedExerciseIds = new HashSet<Guid>();
                var processedSetIds = new HashSet<Guid>();

                // Handle exercises
                foreach (var updateExercise in updateWorkoutDto.Exercises)
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
                            Sets = new List<Set>()
                        };
                        workout.Exercises.Add(exercise);
                        _dbContext.Exercises.Add(exercise);
                        _logger.LogInformation($"Added new exercise: {exercise.ExerciseId}");
                    }
                    else
                    {
                        exercise.Name = updateExercise.Name;
                    }

                    processedExerciseIds.Add(exercise.ExerciseId);

                    // Handle sets
                    foreach (var updateSet in updateExercise.Sets)
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
                            _dbContext.Sets.Add(set);
                            _logger.LogInformation($"Added new set: {set.SetId}");
                        }
                        else
                        {
                            set.Reps = updateSet.Reps;
                            set.Weight = updateSet.Weight;
                        }

                        processedSetIds.Add(set.SetId);
                    }

                    // Delete sets not in updateExercise.Sets
                    var setsToRemove = exercise.Sets.Where(s => !processedSetIds.Contains(s.SetId)).ToList();
                    foreach (var setToRemove in setsToRemove)
                    {
                        exercise.Sets.Remove(setToRemove);
                        _dbContext.Sets.Remove(setToRemove);
                        _logger.LogInformation($"Removed set: {setToRemove.SetId}");
                    }

                    processedSetIds.Clear(); // Clear processed sets for the next exercise
                }

                // Delete exercises not in updateWorkoutDto.Exercises
                var exercisesToRemove = workout.Exercises.Where(e => !processedExerciseIds.Contains(e.ExerciseId)).ToList();
                foreach (var exerciseToRemove in exercisesToRemove)
                {
                    workout.Exercises.Remove(exerciseToRemove);
                    _dbContext.Exercises.Remove(exerciseToRemove);
                    _logger.LogInformation($"Removed exercise: {exerciseToRemove.ExerciseId}");
                }
            }

            await _dbContext.SaveChangesAsync();
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
                if (!string.IsNullOrEmpty(updatedSet.SetId) && !updatedSet.SetId.ToLower().Contains("temp"))
                {
                    var set = exercise.Sets.FirstOrDefault(s => s.SetId.ToString() == updatedSet.SetId);
                    if (set != null)
                    {
                        set.Reps = updatedSet.Reps;
                        set.Weight = updatedSet.Weight;
                    }
                    else
                    {
                        _logger.LogInformation("Adding new set with provided SetId: {setId}", string.IsNullOrEmpty(updatedSet.SetId));
                        var newSet = new Set
                        {
                            SetId = Guid.NewGuid(),
                            ExerciseId = exerciseId,
                            Reps = updatedSet.Reps,
                            Weight = updatedSet.Weight
                        };
                        exercise.Sets.Add(newSet);
                        _dbContext.Sets.Add(newSet);
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
                    _dbContext.Sets.Add(newSet);
                }
            }

            //_dbContext.Exercises.Update(exercise);
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
