using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetSafe.API.DTOs.Pets
{
    public class CreatePetRequest
    {
        public string Name { get; set; } = null!;
        public string Species { get; set; } = null!;
        public string? Breed { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; }
    }
}