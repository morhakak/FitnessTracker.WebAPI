namespace FitnessTracker.WebAPI.Models.DTOs.Workout;

public class ExerciseDto
{
    public string Name { get; set; }
    public List<SetDto> Sets { get; set; } = new List<SetDto>();
}
