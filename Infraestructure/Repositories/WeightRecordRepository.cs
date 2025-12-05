using Microsoft.EntityFrameworkCore;
using PetSafe.Application.Interfaces;
using PetSafe.Config;
using PetSafe.Domain.Models;

namespace PetSafe.Infraestructure.Repositories;

public class WeightRecordRepository : GenericRepository<WeightRecord>, IWeightRecordRepository
{
    public WeightRecordRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<WeightRecord?> GetByIdWithPetAsync(int id)
    {
        return await _dbSet
            .Include(w => w.Pet)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<IEnumerable<WeightRecord>> GetByPetIdAsync(int petId)
    {
        return await _dbSet
            .Where(w => w.PetId == petId)
            .OrderByDescending(w => w.Date)
            .ToListAsync();
    }
}

