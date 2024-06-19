using FitnessTracker.WebAPI.Models.Domain;
using FitnessTracker.WebAPI.Models.DTOs.Workout;

namespace FitnessTracker.WebAPI.Repositories.Interfaces;

public interface IWorkoutRepository
{
    Task<List<Workout>?> GetAllWorkoutsAsync();
    Task<Workout?> GetWorkoutByIdAsync(Guid id);
    Task<List<Workout>> GetAllWorkoutsByUserIdAsync(Guid userId);
    Task<Workout> CreateWorkoutAsync(Workout workout);
    Task<bool> UpdateWorkoutAsync(Guid id,UpdateWorkoutDto updateWorkoutDto);
    Task<bool> DeleteWorkoutAsync(Guid id);
    Task<bool> ToggleLikeWorkoutAsync(Guid workoutId);
}
