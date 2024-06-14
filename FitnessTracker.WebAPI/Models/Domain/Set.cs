using System.Text.Json.Serialization;

namespace FitnessTracker.WebAPI.Models.Domain;

public class Set
{
    public Guid SetId { get; set; }
    public Guid ExerciseId { get; set; }
    public int Reps { get; set; }
    public double Weight { get; set; }
    public string CreatedAt { get; set; } = string.Empty;

    [JsonIgnore]
    public Exercise Exercise { get; set; }
}
