using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetSafe.API.DTOs.Pets;

public class AddWeightRequest
{
    public DateTime Date { get; set; }
    public double Weight { get; set; }
}
