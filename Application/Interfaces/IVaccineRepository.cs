using PetSafe.Domain.Models;
using PetSafe.Infraestructure.Repositories;

namespace PetSafe.Application.Interfaces;

public interface IVaccineRepository : IRepository<Vaccine>
{
    Task<Vaccine?> GetByIdWithPetAsync(int id);
    Task<IEnumerable<Vaccine>> GetByPetIdAsync(int petId);
}

