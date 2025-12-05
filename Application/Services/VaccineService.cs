using PetSafe.API.DTOs.Pets;
using PetSafe.Application.Interfaces;

namespace PetSafe.Application.Services;

public class VaccineService : IVaccineService
{
    private readonly IVaccineRepository _vaccineRepository;

    public VaccineService(IVaccineRepository vaccineRepository)
    {
        _vaccineRepository = vaccineRepository;
    }

    public async Task<VaccineResponse> GetByIdAsync(int id, int ownerId)
    {
        var vaccine = await _vaccineRepository.GetByIdWithPetAsync(id);
        
        if (vaccine == null)
            throw new Exception("Vaccine not found.");

        if (vaccine.Pet.OwnerId != ownerId)
            throw new UnauthorizedAccessException("You don't have permission to access this vaccine.");

        return MapToResponse(vaccine);
    }

    public async Task<IEnumerable<VaccineResponse>> GetByPetIdAsync(int petId, int ownerId)
    {
        var vaccines = await _vaccineRepository.GetByPetIdAsync(petId);
        
        // Verifica se o pet pertence ao usuário através da primeira vacina (se existir)
        if (vaccines.Any())
        {
            var firstVaccine = await _vaccineRepository.GetByIdWithPetAsync(vaccines.First().Id);
            if (firstVaccine?.Pet.OwnerId != ownerId)
                throw new UnauthorizedAccessException("You don't have permission to access these vaccines.");
        }

        return vaccines.Select(MapToResponse);
    }

    public async Task<VaccineResponse> UpdateAsync(int id, UpdateVaccineRequest request, int ownerId)
    {
        var vaccine = await _vaccineRepository.GetByIdWithPetAsync(id);
        
        if (vaccine == null)
            throw new Exception("Vaccine not found.");

        if (vaccine.Pet.OwnerId != ownerId)
            throw new UnauthorizedAccessException("You don't have permission to update this vaccine.");

        // Atualiza apenas campos fornecidos
        if (!string.IsNullOrEmpty(request.Name))
            vaccine.Name = request.Name;
        
        if (request.Date.HasValue)
            vaccine.Date = request.Date.Value;
        
        if (request.Next.HasValue)
            vaccine.Next = request.Next;
        else if (request.Next == null && request.Date.HasValue)
            vaccine.Next = null; // Permite limpar o campo
        
        if (request.Vet != null)
            vaccine.Vet = request.Vet;
        
        if (request.Lot != null)
            vaccine.Lot = request.Lot;

        await _vaccineRepository.UpdateAsync(vaccine);
        
        // Recarrega com relações
        var updatedVaccine = await _vaccineRepository.GetByIdWithPetAsync(id);
        return MapToResponse(updatedVaccine!);
    }

    public async Task DeleteAsync(int id, int ownerId)
    {
        var vaccine = await _vaccineRepository.GetByIdWithPetAsync(id);
        
        if (vaccine == null)
            throw new Exception("Vaccine not found.");

        if (vaccine.Pet.OwnerId != ownerId)
            throw new UnauthorizedAccessException("You don't have permission to delete this vaccine.");

        await _vaccineRepository.DeleteAsync(vaccine);
    }

    private VaccineResponse MapToResponse(Domain.Models.Vaccine vaccine)
    {
        return new VaccineResponse
        {
            Id = vaccine.Id,
            Name = vaccine.Name,
            Date = vaccine.Date,
            Next = vaccine.Next,
            Vet = vaccine.Vet,
            Lot = vaccine.Lot
        };
    }
}

