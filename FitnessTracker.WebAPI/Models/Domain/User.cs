using Microsoft.AspNetCore.Identity;

namespace FitnessTracker.WebAPI.Models.Domain;

public class User : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public ICollection<Workout> Workouts { get; set; } = [];
}
