using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PetSafe.Domain.Models;
using PetSafe.Infraestructure.Repositories;
namespace PetSafe.Application.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}
