using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PetSafe.API.DTOs;

namespace PetSafe.Application.Interfaces;

public interface IUserService
{
    Task<AuthResponse?> LoginAsync(LoginRequest dto);
    Task<AuthResponse> RegisterAsync(RegisterRequest dto);
}
