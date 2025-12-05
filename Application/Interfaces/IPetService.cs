using PetSafe.API.DTOs.Pets;

namespace PetSafe.Application.Interfaces;

public interface IPetService
{
    Task<IEnumerable<PetResponse>> GetAllByOwnerAsync(int ownerId);
    Task<PetResponse?> GetByIdAsync(int id, int ownerId);
    Task<PetResponse> CreateAsync(CreatePetRequest request, int ownerId);
    Task<PetResponse> UpdateAsync(int id, UpdatePetRequest request, int ownerId);
    Task DeleteAsync(int id, int ownerId);
    Task<PetResponse> AddVaccineAsync(int petId, AddVaccineRequest request, int ownerId);
    Task<PetResponse> AddWeightAsync(int petId, AddWeightRequest request, int ownerId);
}

