using FitnessTracker.WebAPI.CustomActionFilters;
using FitnessTracker.WebAPI.Models.Domain;
using FitnessTracker.WebAPI.Models.DTOs.Auth;
using FitnessTracker.WebAPI.Repositories;
using FitnessTracker.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracker.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<SQLAuthRepository> _logger;

    public AuthController(IAuthRepository authRepository, UserManager<User> userManager, ILogger<SQLAuthRepository> logger)
    {
        _authRepository = authRepository;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpPost("Register")]
    [ValidateModel]
    public async Task<ActionResult> Register([FromBody] RegisterUserDto registerRequestDto)
    {
        var result = await _authRepository.RegisterUserAsync(registerRequestDto);

        if (result.Succeeded)
        {
            return Ok(new { message = $"{registerRequestDto.Username} registered successfully" });
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return BadRequest(result.Errors.FirstOrDefault()?.Description);
    }

    [HttpPost("Login")]
    [ValidateModel]
    public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
    {
        var existingUser = await _userManager.FindByNameAsync(loginUserDto.Username);

        if (existingUser == null)
        {
            _logger.LogWarning($"Login attempt failed: User '{loginUserDto.Username}' not found.");
            return NotFound();
        }

        var res = await _authRepository.LoginAsync(existingUser, loginUserDto.Password);

        if (res.IsSuccess is not true)
        {
            return Unauthorized(new { Message = res.ErrorMessage });
        }

        return Ok(new { res.Token, res.UserId, res.Username, res.Email,res.IsAdmin, res.Message});
    }
}
