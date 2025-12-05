using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetSafe.API.DTOs;

public class AuthResponse
{
    public string Token { get; set; } = null!; // JWT
    public AuthUserDto User { get; set; } = null!;
}
