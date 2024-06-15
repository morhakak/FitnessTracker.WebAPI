namespace FitnessTracker.WebAPI.Models.DTOs.Dashboard;

public class UserWithRolesDto
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; }
}
