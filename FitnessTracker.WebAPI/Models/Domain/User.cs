using Microsoft.AspNetCore.Identity;

namespace FitnessTracker.WebAPI.Models.Domain;

public class User : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public List<Workout> Workouts { get; set; } = [];
    public string CreatedAt { get; set; } = string.Empty;
}
