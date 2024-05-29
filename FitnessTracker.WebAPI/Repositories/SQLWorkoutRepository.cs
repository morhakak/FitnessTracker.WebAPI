using FitnessTracker.WebAPI.Data;
using FitnessTracker.WebAPI.Models.Domain;
using FitnessTracker.WebAPI.Models.DTOs.Workout;
using FitnessTracker.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.WebAPI.Repositories;

public class SQLWorkoutRepository : IWorkoutRepository
{
    private readonly FitnessTrackerDbContext _dbContext;
    private readonly UserManager<User> _userManager;

    public SQLWorkoutRepository(FitnessTrackerDbContext dbContext, UserManager<User> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
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
        workout.UpdatedAt = DateTime.UtcNow;

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

        _dbContext.Workouts.Update(workout);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
