using PetSafe.API.DTOs;
using BCrypt.Net;
using PetSafe.Domain.Models;
using PetSafe.Application.Interfaces;

namespace PetSafe.Application.Services;

public class AuthService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthService(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    // ----------------------
    // REGISTER
    // ----------------------
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Verifica duplicidade de email
        var existing = await _userRepository.GetByEmailAsync(request.Email);
        if (existing != null)
            throw new Exception("Email already exists.");

        // Cria usuário
        var user = new User
        {
            Name = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await _userRepository.AddAsync(user);

        // Gera token e retorna resposta
        var token = _jwtService.GenerateToken(user.Id, user.Email);

        return new AuthResponse
        {
            Token = token,
            User = new AuthUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            }
        };
    }

    // ----------------------
    // LOGIN
    // ----------------------
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
            throw new Exception("Invalid email or password.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new Exception("Invalid email or password.");

        var token = _jwtService.GenerateToken(user.Id, user.Email);

        return new AuthResponse
        {
            Token = token,
            User = new AuthUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            }
        };
    }
}
