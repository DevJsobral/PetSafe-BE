using PetSafe.API.DTOs.Pets;

namespace PetSafe.Application.Interfaces;

public interface IVaccineService
{
    Task<VaccineResponse> GetByIdAsync(int id, int ownerId);
    Task<IEnumerable<VaccineResponse>> GetByPetIdAsync(int petId, int ownerId);
    Task<VaccineResponse> UpdateAsync(int id, UpdateVaccineRequest request, int ownerId);
    Task DeleteAsync(int id, int ownerId);
}

