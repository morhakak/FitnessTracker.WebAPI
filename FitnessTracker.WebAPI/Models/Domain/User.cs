using Microsoft.AspNetCore.Identity;

namespace FitnessTracker.WebAPI.Models.Domain;

public class User : IdentityUser
{
    public List<Workout> Workouts { get; set; } = [];
    public string CreatedAt { get; set; } = string.Empty;
    public string? ImageUrl { get; set; } = string.Empty;
}
