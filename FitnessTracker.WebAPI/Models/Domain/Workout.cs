using System.Text.Json.Serialization;

namespace FitnessTracker.WebAPI.Models.Domain;

public class Workout
{
    public Guid WorkoutId { get; set; } 
    public string UserId { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; } 
    public DateTime UpdatedAt { get; set; } 
    public List<Exercise> Exercises { get; set; } = [];
    public bool IsLiked { get; set; }

    // Navigation property
    [JsonIgnore]
    public User User { get; set; }
}
