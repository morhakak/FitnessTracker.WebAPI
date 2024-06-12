namespace FitnessTracker.WebAPI.Models.DTOs.Workout;

public class AddSetDto
{
    public Guid WorkoutId { get; set; }
    public Guid ExerciseId { get; set; }
}
