using PetSafe.Domain.Models;
using PetSafe.Infraestructure.Repositories;

namespace PetSafe.Application.Interfaces;

public interface IWeightRecordRepository : IRepository<WeightRecord>
{
    Task<WeightRecord?> GetByIdWithPetAsync(int id);
    Task<IEnumerable<WeightRecord>> GetByPetIdAsync(int petId);
}

