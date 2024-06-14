namespace FitnessTracker.WebAPI.Models.DTOs.Workout;

public class UpdateSetDto
{
    public string? SetId { get; set; } 
    public int Reps { get; set; }
    public double Weight { get; set; }
}
