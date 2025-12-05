using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetSafe.API.DTOs.Pets;
using PetSafe.Application.Interfaces;

namespace PetSafe.API.Controllers;

[ApiController]
[Route("api/pets/{petId}/weights")]
[Authorize]
public class WeightRecordsController : ControllerBase
{
    private readonly IWeightRecordService _weightRecordService;

    public WeightRecordsController(IWeightRecordService weightRecordService)
    {
        _weightRecordService = weightRecordService;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user token.");
        return userId;
    }

    // GET: api/pets/{petId}/weights
    [HttpGet]
    public async Task<IActionResult> GetByPetId(int petId)
    {
        try
        {
            var userId = GetUserId();
            var weightRecords = await _weightRecordService.GetByPetIdAsync(petId, userId);
            return Ok(weightRecords);
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

    // GET: api/pets/{petId}/weights/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int petId, int id)
    {
        try
        {
            var userId = GetUserId();
            var weightRecord = await _weightRecordService.GetByIdAsync(id, userId);
            return Ok(weightRecord);
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

    // PUT: api/pets/{petId}/weights/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int petId, int id, [FromBody] UpdateWeightRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var weightRecord = await _weightRecordService.UpdateAsync(id, request, userId);
            return Ok(weightRecord);
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

    // DELETE: api/pets/{petId}/weights/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int petId, int id)
    {
        try
        {
            var userId = GetUserId();
            await _weightRecordService.DeleteAsync(id, userId);
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

