using System.Collections.Generic;

namespace FitnessTracker.WebAPI.Models.Domain;

public class Exercise
{
    public Guid ExerciseId { get; set; }
    public Guid WorkoutId { get; set; }
    public string Name { get; set; }
    public ICollection<Set> Sets { get; set; }
    public Workout Workout { get; set; }
}
