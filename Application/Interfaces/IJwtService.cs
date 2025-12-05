using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PetSafe.Domain.Models;

namespace PetSafe.Application.Interfaces;

    public interface IJwtService
    {
        string GenerateToken(int userId, string email);
    }
