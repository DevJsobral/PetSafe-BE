using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetSafe.API.DTOs.Pets;

public class AddVaccineRequest
{
    public string Name { get; set; } = null!;
    public DateTime Date { get; set; }
    public DateTime? Next { get; set; }
    public string? Vet { get; set; }
    public string? Lot { get; set; }
}
