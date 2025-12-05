using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PetSafe.API.DTOs;
using PetSafe.Application.Interfaces;
using PetSafe.Application.Services;
using PetSafe.Infraestructure.Repositories;

namespace PetSafe.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _service;

    public AuthController(IUserService service)
    {
        _service = service;
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _service.LoginAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            // Padrão: 401 em login inválido
            return Unauthorized(new { message = ex.Message });
        }
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var result = await _service.RegisterAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            // Padrão: 400 em erro de registro
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize()]
    [HttpGet("ping")]
    public string Teste()
    {
        return "AuthController is working!";
    }
}
