namespace PetSafe.Domain.Models;

public class WeightRecord
{
    public int Id { get; set; }

    public DateTime Date { get; set; }
    public double Weight { get; set; }

    // FK
    public int PetId { get; set; }
    public Pet Pet { get; set; } = null!;
}