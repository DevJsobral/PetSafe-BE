using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetSafe.API.DTOs.Pets;
using PetSafe.Application.Interfaces;

namespace PetSafe.API.Controllers;

[ApiController]
[Route("api/pets/{petId}/vaccines")]
[Authorize]
public class VaccinesController : ControllerBase
{
    private readonly IVaccineService _vaccineService;

    public VaccinesController(IVaccineService vaccineService)
    {
        _vaccineService = vaccineService;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user token.");
        return userId;
    }

    // GET: api/pets/{petId}/vaccines
    [HttpGet]
    public async Task<IActionResult> GetByPetId(int petId)
    {
        try
        {
            var userId = GetUserId();
            var vaccines = await _vaccineService.GetByPetIdAsync(petId, userId);
            return Ok(vaccines);
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

    // GET: api/pets/{petId}/vaccines/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int petId, int id)
    {
        try
        {
            var userId = GetUserId();
            var vaccine = await _vaccineService.GetByIdAsync(id, userId);
            return Ok(vaccine);
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

    // PUT: api/pets/{petId}/vaccines/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int petId, int id, [FromBody] UpdateVaccineRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var vaccine = await _vaccineService.UpdateAsync(id, request, userId);
            return Ok(vaccine);
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

    // DELETE: api/pets/{petId}/vaccines/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int petId, int id)
    {
        try
        {
            var userId = GetUserId();
            await _vaccineService.DeleteAsync(id, userId);
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
}

