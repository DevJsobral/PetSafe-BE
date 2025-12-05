using Microsoft.EntityFrameworkCore;
using PetSafe.Application.Interfaces;
using PetSafe.Config;
using PetSafe.Domain.Models;

namespace PetSafe.Infraestructure.Repositories;

public class VaccineRepository : GenericRepository<Vaccine>, IVaccineRepository
{
    public VaccineRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Vaccine?> GetByIdWithPetAsync(int id)
    {
        return await _dbSet
            .Include(v => v.Pet)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<IEnumerable<Vaccine>> GetByPetIdAsync(int petId)
    {
        return await _dbSet
            .Where(v => v.PetId == petId)
            .OrderByDescending(v => v.Date)
            .ToListAsync();
    }
}

