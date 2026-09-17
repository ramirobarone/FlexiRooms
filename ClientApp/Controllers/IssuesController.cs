using System.Security.Claims;
using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientApp.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class IssuesController(IIssueService issueService, ILogger<IssuesController> logger) : ControllerBase
{
    [HttpGet, Route(nameof(GetIssueTypes))]
    public async Task<IActionResult> GetIssueTypes(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<IssueTypeDto> issueTypes = await issueService.GetIssueTypesAsync(cancellationToken);
        return Ok(issueTypes);
    }

    [HttpGet, Route(nameof(GetMyIssues))]
    public async Task<IActionResult> GetMyIssues(CancellationToken cancellationToken)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        IReadOnlyCollection<IssueDto> issues = await issueService.GetMyIssuesAsync(userId, cancellationToken);
        return Ok(issues);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateIssue([FromForm] int tipoDeReclamo, [FromForm] string texto, [FromForm] IFormFile? imagen, CancellationToken cancellationToken)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(texto))
            return BadRequest("El texto del reclamo es obligatorio.");

        IssueUploadFile? mappedImage = null;
        if (imagen is not null && imagen.Length > 0)
        {
            await using MemoryStream memoryStream = new();
            await imagen.CopyToAsync(memoryStream, cancellationToken);
            mappedImage = new IssueUploadFile(imagen.FileName, memoryStream.ToArray());
        }

        try
        {
            IssueDto created = await issueService.CreateIssueAsync(userId, tipoDeReclamo, texto, mappedImage, cancellationToken);
            logger.LogInformation("Created issue {IssueId} for user {UserId}", created.Id, userId);
            return Ok(created);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid issue creation request for user {UserId}", userId);
            return BadRequest(ex.Message);
        }
    }
}
