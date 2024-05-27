using FitnessTracker.WebAPI.Models.Domain;

namespace FitnessTracker.WebAPI.Repositories.Interfaces;

public interface IWorkoutRepository
{
    Task<List<Workout>?> GetAllWorkoutsAsync();
    Task<Workout?> GetWorkoutByIdAsync(Guid id);
    Task<List<Workout>> GetAllWorkoutsByUserIdAsync(Guid userId);
    Task<Workout> CreateWorkoutAsync(Workout workout);
    Task<Workout?> UpdateWorkoutAsync(Workout workout);
    Task<bool> DeleteWorkoutAsync(Guid id);
}
