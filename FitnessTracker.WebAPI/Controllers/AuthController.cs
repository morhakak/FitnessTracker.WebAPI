﻿using FitnessTracker.WebAPI.CustomActionFilters;
using FitnessTracker.WebAPI.Models.DTOs.Auth;
using FitnessTracker.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracker.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;

    public AuthController(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    [HttpPost("Register")]
    [ValidateModel]
    public async Task<ActionResult> Register([FromBody] RegisterUserDto registerRequestDto)
    {
        var result = await _authRepository.RegisterUserAsync(registerRequestDto);

        if (result.Succeeded)
        {
            return Ok(new { message = $"{registerRequestDto.FirstName} {registerRequestDto.LastName} registered successfully" });
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return BadRequest(ModelState);
    }

    [HttpPost("Login")]
    [ValidateModel]
    public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
    {
        var token = await _authRepository.LoginAsync(loginUserDto.Username, loginUserDto.Password);

        if (token is null)
        {
            return Unauthorized("Invalid login attempt");
        }

        return Ok($"{loginUserDto.Username} logged in successfully");
    }
}