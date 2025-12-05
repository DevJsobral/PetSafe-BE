using PetSafe.Domain.Models;
using PetSafe.Infraestructure.Repositories;

namespace PetSafe.Application.Interfaces;

public interface IPetRepository : IRepository<Pet>
{
    Task<IEnumerable<Pet>> GetByOwnerIdAsync(int ownerId);
    Task<Pet?> GetByIdWithRelationsAsync(int id);
}

