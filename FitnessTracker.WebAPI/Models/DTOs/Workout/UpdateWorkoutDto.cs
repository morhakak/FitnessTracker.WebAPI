namespace FitnessTracker.WebAPI.Models.DTOs.Workout;

public class UpdateWorkoutDto
{
    public string Name { get; set; }
    public bool IsLiked { get; set; }
    public string UpdatedAt { get; set; }
    public List<UpdateExerciseDto> Exercises { get; set; }
}
