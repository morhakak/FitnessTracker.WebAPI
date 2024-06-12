using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.WebAPI.Models.DTOs.Workout;

public class CreateWorkoutRequest
{
    [Required]
    public string Name { get; set; }
}
