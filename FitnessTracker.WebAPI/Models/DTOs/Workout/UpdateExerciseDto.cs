namespace FitnessTracker.WebAPI.Models.DTOs.Workout;

public class UpdateExerciseDto
{
    public Guid? ExerciseId { get; set; }
    public string Name { get; set; }
    public List<UpdateSetDto> Sets { get; set; } = new();
}
