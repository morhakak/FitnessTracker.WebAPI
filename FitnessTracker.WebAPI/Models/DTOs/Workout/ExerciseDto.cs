using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.WebAPI.Models.DTOs.Workout;

public class ExerciseDto
{
    [Required]
    public string Name { get; set; }
    [Required]
    public List<SetDto> Sets { get; set; } = new List<SetDto>();
}
