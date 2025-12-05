namespace PetSafe.API.DTOs.Pets;

public class UpdateVaccineRequest
{
    public string? Name { get; set; }
    public DateTime? Date { get; set; }
    public DateTime? Next { get; set; }
    public string? Vet { get; set; }
    public string? Lot { get; set; }
}

