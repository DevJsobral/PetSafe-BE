using Microsoft.EntityFrameworkCore;
using PetSafe.Application.Interfaces;
using PetSafe.Config;
using PetSafe.Domain.Models;

namespace PetSafe.Infraestructure.Repositories;

public class PetRepository : GenericRepository<Pet>, IPetRepository
{
    public PetRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Pet>> GetByOwnerIdAsync(int ownerId)
    {
        return await _dbSet
            .Where(p => p.OwnerId == ownerId)
            .Include(p => p.Vaccines)
            .Include(p => p.Weights)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Pet?> GetByIdWithRelationsAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Vaccines)
            .Include(p => p.Weights)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}

