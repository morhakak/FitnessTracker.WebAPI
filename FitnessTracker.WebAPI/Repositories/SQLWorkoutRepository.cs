using FitnessTracker.WebAPI.Data;
using FitnessTracker.WebAPI.Models.Domain;
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

    public async Task<Workout?> UpdateWorkoutAsync(Workout workout)
    {
        var existingWorkout = await _dbContext.Workouts.FindAsync(workout.WorkoutId);

        if (existingWorkout == null) return null;

        //create new workout and update it

       await _dbContext.SaveChangesAsync();

        return null; //temp
    }
}
