using PetSafe.API.DTOs.Pets;

namespace PetSafe.API.DTOs;

public class PublicUserResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? EmergencyPhone { get; set; }
    public List<PublicPetResponse> Pets { get; set; } = new();
}

public class PublicPetResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Species { get; set; } = null!;
    public string? Breed { get; set; }
    public int Age { get; set; }
    public double Weight { get; set; }
    public string? PhotoUrl { get; set; }
    public List<VaccineResponse> Vaccines { get; set; } = new();
    public List<WeightRecordResponse> Weights { get; set; } = new();
}

