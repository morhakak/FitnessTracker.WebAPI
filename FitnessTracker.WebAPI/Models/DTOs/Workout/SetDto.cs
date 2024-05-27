using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.WebAPI.Models.DTOs.Workout;

public class SetDto
{
    [Required]
    public int Reps { get; set; }
    [Required]
    public double Weight { get; set; }
}
