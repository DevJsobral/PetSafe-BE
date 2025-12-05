using Microsoft.AspNetCore.Mvc;
using PetSafe.API.DTOs;
using PetSafe.API.DTOs.Pets;
using PetSafe.Application.Interfaces;
using PetSafe.Domain.Models;

namespace PetSafe.API.Controllers;

[ApiController]
[Route("api/public")]
public class PublicController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IPetRepository _petRepository;

    public PublicController(IUserRepository userRepository, IPetRepository petRepository)
    {
        _userRepository = userRepository;
        _petRepository = petRepository;
    }

    // GET: api/public/user/{userId}
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetPublicUserProfile(int userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
                return NotFound(new { message = "User not found." });

            var pets = await _petRepository.GetByOwnerIdAsync(userId);
            
            var response = new PublicUserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                EmergencyPhone = user.EmergencyPhone,
                Pets = pets.Select(p => MapToPublicPetResponse(p)).ToList()
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // GET: api/public/pet/{petId}
    [HttpGet("pet/{petId}")]
    public async Task<IActionResult> GetPublicPet(int petId)
    {
        try
        {
            var pet = await _petRepository.GetByIdWithRelationsAsync(petId);
            
            if (pet == null)
                return NotFound(new { message = "Pet not found." });

            var owner = await _userRepository.GetByIdAsync(pet.OwnerId);
            
            var response = new
            {
                Pet = MapToPublicPetResponse(pet),
                Owner = owner != null ? new
                {
                    Id = owner.Id,
                    Name = owner.Name,
                    Email = owner.Email,
                    EmergencyPhone = owner.EmergencyPhone
                } : null
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    private PublicPetResponse MapToPublicPetResponse(Pet pet)
    {
        return new PublicPetResponse
        {
            Id = pet.Id,
            Name = pet.Name,
            Species = pet.Species,
            Breed = pet.Breed,
            Age = pet.Age,
            Weight = pet.Weight,
            PhotoUrl = pet.PhotoUrl,
            Vaccines = pet.Vaccines?.Select(v => new VaccineResponse
            {
                Id = v.Id,
                Name = v.Name,
                Date = v.Date,
                Next = v.Next,
                Vet = v.Vet,
                Lot = v.Lot
            }).ToList() ?? new List<VaccineResponse>(),
            Weights = pet.Weights?.Select(w => new WeightRecordResponse
            {
                Id = w.Id,
                Date = w.Date,
                Weight = w.Weight
            }).ToList() ?? new List<WeightRecordResponse>()
        };
    }
}

