using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetSafe.API.DTOs.Pets;
using PetSafe.Application.Interfaces;

namespace PetSafe.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PetsController : ControllerBase
{
    private readonly IPetService _petService;

    public PetsController(IPetService petService)
    {
        _petService = petService;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user token.");
        return userId;
    }

    // GET: api/pets
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var userId = GetUserId();
            var pets = await _petService.GetAllByOwnerAsync(userId);
            return Ok(pets);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // GET: api/pets/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var userId = GetUserId();
            var pet = await _petService.GetByIdAsync(id, userId);
            
            if (pet == null)
                return NotFound(new { message = "Pet not found." });

            return Ok(pet);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST: api/pets
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePetRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var pet = await _petService.CreateAsync(request, userId);
            return CreatedAtAction(nameof(GetById), new { id = pet.Id }, pet);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/pets/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePetRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var pet = await _petService.UpdateAsync(id, request, userId);
            return Ok(pet);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE: api/pets/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var userId = GetUserId();
            await _petService.DeleteAsync(id, userId);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST: api/pets/{id}/vaccines
    [HttpPost("{id}/vaccines")]
    public async Task<IActionResult> AddVaccine(int id, [FromBody] AddVaccineRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var pet = await _petService.AddVaccineAsync(id, request, userId);
            return Ok(pet);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST: api/pets/{id}/weights
    [HttpPost("{id}/weights")]
    public async Task<IActionResult> AddWeight(int id, [FromBody] AddWeightRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var pet = await _petService.AddWeightAsync(id, request, userId);
            return Ok(pet);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

