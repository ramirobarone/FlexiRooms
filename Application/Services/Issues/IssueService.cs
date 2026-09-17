using Application.Interfaces;
using Application.Models;
using Infrastructure.Context;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Issues;

public class IssueService(FlexiRoomsContext flexiRoomsContext) : IIssueService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    private const int MaxFileSizeBytes = 5 * 1024 * 1024;

    public async Task<IssueDto> CreateIssueAsync(string applicationUserId, int tipoDeReclamo, string texto, IssueUploadFile? image, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(applicationUserId))
            throw new ArgumentException("User id is invalid.", nameof(applicationUserId));

        if (string.IsNullOrWhiteSpace(texto))
            throw new ArgumentException("El texto del reclamo es obligatorio.", nameof(texto));

        bool issueTypeExists = await flexiRoomsContext.IssuesTypes.AnyAsync(x => x.Id == tipoDeReclamo, cancellationToken);
        if (!issueTypeExists)
            throw new ArgumentException("El tipo de reclamo indicado no existe.", nameof(tipoDeReclamo));

        Issue issue = new()
        {
            TipoDeReclamo = tipoDeReclamo,
            Texto = texto,
            Estado = "Pendiente",
            ApplicationUserId = applicationUserId,
            CreatedAtUtc = DateTime.UtcNow
        };

        if (image is not null)
        {
            ValidateFile(image);

            string issuesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "issues");
            Directory.CreateDirectory(issuesDirectory);

            string extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            string newFileName = $"{Guid.NewGuid()}{extension}";
            string physicalPath = Path.Combine(issuesDirectory, newFileName);

            await File.WriteAllBytesAsync(physicalPath, image.Content, cancellationToken);

            issue.Imagen = $"/issues/{newFileName}";
        }

        flexiRoomsContext.Issues.Add(issue);
        await flexiRoomsContext.SaveChangesAsync(cancellationToken);

        Issue created = await flexiRoomsContext.Issues
            .Include(x => x.IssueType)
            .FirstAsync(x => x.Id == issue.Id, cancellationToken);

        return created;
    }

    public async Task<IReadOnlyCollection<IssueDto>> GetMyIssuesAsync(string applicationUserId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(applicationUserId))
            throw new ArgumentException("User id is invalid.", nameof(applicationUserId));

        List<Issue> issues = await flexiRoomsContext.Issues
            .Include(x => x.IssueType)
            .Where(x => x.ApplicationUserId == applicationUserId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return issues.Select(x => (IssueDto)x).ToList();
    }

    public async Task<IReadOnlyCollection<IssueTypeDto>> GetIssueTypesAsync(CancellationToken cancellationToken = default)
    {
        List<IssueType> issueTypes = await flexiRoomsContext.IssuesTypes
            .OrderBy(x => x.Issue)
            .ToListAsync(cancellationToken);

        return issueTypes.Select(x => (IssueTypeDto)x).ToList();
    }

    private static void ValidateFile(IssueUploadFile file)
    {
        if (string.IsNullOrWhiteSpace(file.FileName))
            throw new ArgumentException("El nombre del archivo es obligatorio.", nameof(file));

        if (file.Content is null || file.Content.Length == 0)
            throw new ArgumentException("El contenido de la imagen es obligatorio.", nameof(file));

        if (file.Content.Length > MaxFileSizeBytes)
            throw new ArgumentException("La imagen supera el tamaño máximo permitido.", nameof(file));

        string extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
            throw new ArgumentException("El formato de imagen no es compatible.", nameof(file));
    }
}
