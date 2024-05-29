namespace FitnessTracker.WebAPI.Models.DTOs.Workout;

public class UpdateWorkoutDto
{
    public string Name { get; set; }
    public List<UpdateExerciseDto> Exercises { get; set; } = new();
    public bool IsLiked { get; set; }
}
