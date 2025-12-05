using PetSafe.API.DTOs.Pets;
using PetSafe.Application.Interfaces;
using PetSafe.Domain.Models;

namespace PetSafe.Application.Services;

public class PetService : IPetService
{
    private readonly IPetRepository _petRepository;

    public PetService(IPetRepository petRepository)
    {
        _petRepository = petRepository;
    }

    public async Task<IEnumerable<PetResponse>> GetAllByOwnerAsync(int ownerId)
    {
        var pets = await _petRepository.GetByOwnerIdAsync(ownerId);
        return pets.Select(MapToResponse);
    }

    public async Task<PetResponse?> GetByIdAsync(int id, int ownerId)
    {
        var pet = await _petRepository.GetByIdWithRelationsAsync(id);

        if (pet == null)
            throw new Exception("Pet not found.");

        if (pet.OwnerId != ownerId)
            throw new UnauthorizedAccessException("You don't have permission to access this pet.");

        return MapToResponse(pet);
    }

    public async Task<PetResponse> CreateAsync(CreatePetRequest request, int ownerId)
    {
        var pet = new Pet
        {
            Name = request.Name,
            Species = request.Species,
            Breed = request.Breed,
            Age = request.Age,
            Weight = request.Weight,
            OwnerId = ownerId
        };

        await _petRepository.AddAsync(pet);

        // Recarrega com relações
        var createdPet = await _petRepository.GetByIdWithRelationsAsync(pet.Id);
        return MapToResponse(createdPet!);
    }

    public async Task<PetResponse> UpdateAsync(int id, UpdatePetRequest request, int ownerId)
    {
        var pet = await _petRepository.GetByIdAsync(id);

        if (pet == null)
            throw new Exception("Pet not found.");

        if (pet.OwnerId != ownerId)
            throw new UnauthorizedAccessException("You don't have permission to update this pet.");

        // ---- Atualiza apenas a imagem ----
        if (!string.IsNullOrEmpty(request.PhotoUrl))
            pet.PhotoUrl = request.PhotoUrl;

        await _petRepository.UpdateAsync(pet);

        // Recarrega com relações
        var updatedPet = await _petRepository.GetByIdWithRelationsAsync(pet.Id);
        return MapToResponse(updatedPet!);
    }

    public async Task DeleteAsync(int id, int ownerId)
    {
        var pet = await _petRepository.GetByIdAsync(id);

        if (pet == null)
            throw new Exception("Pet not found.");

        if (pet.OwnerId != ownerId)
            throw new UnauthorizedAccessException("You don't have permission to delete this pet.");

        await _petRepository.DeleteAsync(pet);
    }

    public async Task<PetResponse> AddVaccineAsync(int petId, AddVaccineRequest request, int ownerId)
    {
        var pet = await _petRepository.GetByIdWithRelationsAsync(petId);

        if (pet == null)
            throw new Exception("Pet not found.");

        if (pet.OwnerId != ownerId)
            throw new UnauthorizedAccessException("You don't have permission to add vaccines to this pet.");

        var vaccine = new Vaccine
        {
            Name = request.Name,
            Date = request.Date,
            Next = request.Next,
            Vet = request.Vet,
            Lot = request.Lot,
            PetId = petId
        };

        pet.Vaccines.Add(vaccine);
        await _petRepository.UpdateAsync(pet);

        // Recarrega com relações atualizadas
        var updatedPet = await _petRepository.GetByIdWithRelationsAsync(petId);
        return MapToResponse(updatedPet!);
    }

    public async Task<PetResponse> AddWeightAsync(int petId, AddWeightRequest request, int ownerId)
    {
        var pet = await _petRepository.GetByIdWithRelationsAsync(petId);

        if (pet == null)
            throw new Exception("Pet not found.");

        if (pet.OwnerId != ownerId)
            throw new UnauthorizedAccessException("You don't have permission to add weight records to this pet.");

        var weightRecord = new WeightRecord
        {
            Date = request.Date,
            Weight = request.Weight,
            PetId = petId
        };

        pet.Weights.Add(weightRecord);

        // Atualiza o peso atual do pet se for o registro mais recente
        var weights = pet.Weights.OrderByDescending(w => w.Date).ToList();
        if (weights.Any() && weights[0].Date >= request.Date)
        {
            pet.Weight = request.Weight;
        }

        await _petRepository.UpdateAsync(pet);

        // Recarrega com relações atualizadas
        var updatedPet = await _petRepository.GetByIdWithRelationsAsync(petId);
        return MapToResponse(updatedPet!);
    }

    private PetResponse MapToResponse(Pet pet)
    {
        return new PetResponse
        {
            Id = pet.Id,
            Name = pet.Name,
            Species = pet.Species,
            Breed = pet.Breed,
            Age = pet.Age,
            Weight = pet.Weight,
            PhotoUrl = pet.PhotoUrl,
            Vaccines = pet.Vaccines.Select(v => new VaccineResponse
            {
                Id = v.Id,
                Name = v.Name,
                Date = v.Date,
                Next = v.Next,
                Vet = v.Vet,
                Lot = v.Lot
            }).ToList(),
            Weights = pet.Weights.Select(w => new WeightRecordResponse
            {
                Id = w.Id,
                Date = w.Date,
                Weight = w.Weight
            }).ToList()
        };
    }
}

