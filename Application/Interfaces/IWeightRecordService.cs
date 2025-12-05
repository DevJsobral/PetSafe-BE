using PetSafe.API.DTOs.Pets;

namespace PetSafe.Application.Interfaces;

public interface IWeightRecordService
{
    Task<WeightRecordResponse> GetByIdAsync(int id, int ownerId);
    Task<IEnumerable<WeightRecordResponse>> GetByPetIdAsync(int petId, int ownerId);
    Task<WeightRecordResponse> UpdateAsync(int id, UpdateWeightRequest request, int ownerId);
    Task DeleteAsync(int id, int ownerId);
}

