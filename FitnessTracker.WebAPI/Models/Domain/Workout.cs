using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FitnessTracker.WebAPI.Models.Domain;

public class Workout
{
    public Guid WorkoutId { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; } = string.Empty;
    public List<Exercise> Exercises { get; set; } = [];
    public bool IsLiked { get; set; }
    [Timestamp]
    public byte[] RowVersion { get; set; }

    [JsonIgnore]
    public User User { get; set; }
}
