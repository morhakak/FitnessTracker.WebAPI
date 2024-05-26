namespace FitnessTracker.WebAPI.Models.Domain;

public class Workout
{
    public Guid WorkoutId { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public List<Exercise> Exercises { get; set; } = [];

    // Navigation property
    public User User { get; set; }
}
