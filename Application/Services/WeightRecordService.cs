using PetSafe.API.DTOs.Pets;
using PetSafe.Application.Interfaces;

namespace PetSafe.Application.Services;

public class WeightRecordService : IWeightRecordService
{
    private readonly IWeightRecordRepository _weightRecordRepository;
    private readonly IPetRepository _petRepository;

    public WeightRecordService(IWeightRecordRepository weightRecordRepository, IPetRepository petRepository)
    {
        _weightRecordRepository = weightRecordRepository;
        _petRepository = petRepository;
    }

    public async Task<WeightRecordResponse> GetByIdAsync(int id, int ownerId)
    {
        var weightRecord = await _weightRecordRepository.GetByIdWithPetAsync(id);
        
        if (weightRecord == null)
            throw new Exception("Weight record not found.");

        if (weightRecord.Pet.OwnerId != ownerId)
            throw new UnauthorizedAccessException("You don't have permission to access this weight record.");

        return MapToResponse(weightRecord);
    }

    public async Task<IEnumerable<WeightRecordResponse>> GetByPetIdAsync(int petId, int ownerId)
    {
        var weightRecords = await _weightRecordRepository.GetByPetIdAsync(petId);
        
        // Verifica se o pet pertence ao usuário através do primeiro registro (se existir)
        if (weightRecords.Any())
        {
            var firstRecord = await _weightRecordRepository.GetByIdWithPetAsync(weightRecords.First().Id);
            if (firstRecord?.Pet.OwnerId != ownerId)
                throw new UnauthorizedAccessException("You don't have permission to access these weight records.");
        }

        return weightRecords.Select(MapToResponse);
    }

    public async Task<WeightRecordResponse> UpdateAsync(int id, UpdateWeightRequest request, int ownerId)
    {
        var weightRecord = await _weightRecordRepository.GetByIdWithPetAsync(id);
        
        if (weightRecord == null)
            throw new Exception("Weight record not found.");

        if (weightRecord.Pet.OwnerId != ownerId)
            throw new UnauthorizedAccessException("You don't have permission to update this weight record.");

        // Atualiza apenas campos fornecidos
        if (request.Date.HasValue)
            weightRecord.Date = request.Date.Value;
        
        if (request.Weight.HasValue)
            weightRecord.Weight = request.Weight.Value;

        await _weightRecordRepository.UpdateAsync(weightRecord);
        
        // Atualiza o peso atual do pet se este for o registro mais recente
        var pet = await _petRepository.GetByIdWithRelationsAsync(weightRecord.PetId);
        if (pet != null)
        {
            var weights = pet.Weights.OrderByDescending(w => w.Date).ToList();
            if (weights.Any() && weights[0].Id == id)
            {
                pet.Weight = weightRecord.Weight;
                await _petRepository.UpdateAsync(pet);
            }
        }
        
        // Recarrega com relações
        var updatedRecord = await _weightRecordRepository.GetByIdWithPetAsync(id);
        return MapToResponse(updatedRecord!);
    }

    public async Task DeleteAsync(int id, int ownerId)
    {
        var weightRecord = await _weightRecordRepository.GetByIdWithPetAsync(id);
        
        if (weightRecord == null)
            throw new Exception("Weight record not found.");

        if (weightRecord.Pet.OwnerId != ownerId)
            throw new UnauthorizedAccessException("You don't have permission to delete this weight record.");

        var petId = weightRecord.PetId;
        
        await _weightRecordRepository.DeleteAsync(weightRecord);
        
        // Atualiza o peso atual do pet se o registro deletado era o mais recente
        var pet = await _petRepository.GetByIdWithRelationsAsync(petId);
        if (pet != null && pet.Weights.Any())
        {
            var latestWeight = pet.Weights.OrderByDescending(w => w.Date).First();
            pet.Weight = latestWeight.Weight;
            await _petRepository.UpdateAsync(pet);
        }
    }

    private WeightRecordResponse MapToResponse(Domain.Models.WeightRecord weightRecord)
    {
        return new WeightRecordResponse
        {
            Id = weightRecord.Id,
            Date = weightRecord.Date,
            Weight = weightRecord.Weight
        };
    }
}

