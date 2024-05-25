namespace FitnessTracker.WebAPI.Models.Domain;

public class Workout
{
    public Guid WorkoutId { get; set; }
    public string UserId { get; set; } // This will be a foreign key to ApplicationUser
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public ICollection<Exercise> Exercises { get; set; }

    // Navigation property
    public User User { get; set; }
}
