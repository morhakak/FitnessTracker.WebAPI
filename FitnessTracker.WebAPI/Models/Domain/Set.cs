namespace FitnessTracker.WebAPI.Models.Domain;

public class Set
{
    public Guid SetId { get; set; }
    public Guid ExerciseId { get; set; }
    public int Reps { get; set; }
    public double Weight { get; set; }
    public Exercise Exercise { get; set; } = new Exercise();
}
