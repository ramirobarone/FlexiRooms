using System.Security.Claims;
using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientApp.Controllers;

[Authorize(Policy = "HotelManagement")]
[ApiController]
[Route("api/[controller]")]
public class HotelMaintenanceController(IHotelMaintenanceService maintenanceService, ILogger<HotelMaintenanceController> logger) : ControllerBase
{
    [HttpGet(nameof(GetByHotel))]
    public async Task<IActionResult> GetByHotel([FromQuery] int hotelId, CancellationToken cancellationToken)
    {
        try
        {
            IReadOnlyCollection<HotelMaintenanceDto> maintenances = await maintenanceService.GetByHotelIdAsync(hotelId, cancellationToken);
            return Ok(maintenances);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid GetByHotel request for hotel {HotelId}", hotelId);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet(nameof(GetById))]
    public async Task<IActionResult> GetById([FromQuery] int id, CancellationToken cancellationToken)
    {
        try
        {
            HotelMaintenanceDto maintenance = await maintenanceService.GetByIdAsync(id, cancellationToken);
            return Ok(maintenance);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid GetById request for maintenance {MaintenanceId}", id);
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Maintenance {MaintenanceId} not found", id);
            return NotFound(ex.Message);
        }
    }

    [HttpGet(nameof(GetMaintenanceTypes))]
    public async Task<IActionResult> GetMaintenanceTypes(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<MaintenanceTypeDto> types = await maintenanceService.GetMaintenanceTypesAsync(cancellationToken);
        return Ok(types);
    }

    [HttpPost(nameof(Create))]
    public async Task<IActionResult> Create([FromBody] CreateHotelMaintenanceDto request, CancellationToken cancellationToken)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        try
        {
            HotelMaintenanceDto created = await maintenanceService.CreateAsync(request, userId, cancellationToken);
            logger.LogInformation("Created hotel maintenance {MaintenanceId} by user {UserId}", created.Id, userId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid create request for user {UserId}", userId);
            return BadRequest(ex.Message);
        }
    }

    [HttpPut(nameof(Update))]
    public async Task<IActionResult> Update([FromBody] UpdateHotelMaintenanceDto request, CancellationToken cancellationToken)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        try
        {
            HotelMaintenanceDto updated = await maintenanceService.UpdateAsync(request, userId, cancellationToken);
            logger.LogInformation("Updated hotel maintenance {MaintenanceId} by user {UserId}", updated.Id, userId);
            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid update request for maintenance {MaintenanceId}", request.Id);
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Maintenance {MaintenanceId} not found for update", request.Id);
            return NotFound(ex.Message);
        }
    }

    [HttpDelete(nameof(Delete))]
    public async Task<IActionResult> Delete([FromQuery] int id, CancellationToken cancellationToken)
    {
        try
        {
            await maintenanceService.DeleteAsync(id, cancellationToken);
            logger.LogInformation("Deleted hotel maintenance {MaintenanceId}", id);
            return Accepted();
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid delete request for maintenance {MaintenanceId}", id);
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Maintenance {MaintenanceId} not found for delete", id);
            return NotFound(ex.Message);
        }
    }
}
