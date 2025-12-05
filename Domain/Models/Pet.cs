using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PetSafe.Domain.Models;

public class Pet
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public string Species { get; set; } = null!;
    public string? Breed { get; set; }

    public int Age { get; set; }
    public double Weight { get; set; } 

    // Dono
    public int OwnerId { get; set; }
    public User Owner { get; set; } = null!;

    // Coleções
    public ICollection<Vaccine> Vaccines { get; set; } = new List<Vaccine>();
    public ICollection<WeightRecord> Weights { get; set; } = new List<WeightRecord>();

    // Futuro (opcional): foto
    public string? PhotoUrl { get; set; }
}
