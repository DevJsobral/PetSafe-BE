namespace PetSafe.Domain.Models;

public class Vaccine
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public DateTime Date { get; set; }        // última aplicação
    public DateTime? Next { get; set; }       // próxima dose (pode ser null)

    public string? Vet { get; set; }
    public string? Lot { get; set; }

    // FK
    public int PetId { get; set; }
    public Pet Pet { get; set; } = null!;
}