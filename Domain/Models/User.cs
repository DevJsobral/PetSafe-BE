namespace PetSafe.Domain.Models;

public class User
{
    public int Id { get; set; } 
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? EmergencyPhone { get; set; }

    // Navegação
    public ICollection<Pet> Pets { get; set; } = new List<Pet>();
}