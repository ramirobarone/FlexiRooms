using Application.Models;

namespace Application.Interfaces;

public interface IIssueService
{
    Task<IssueDto> CreateIssueAsync(string applicationUserId, int tipoDeReclamo, string texto, IssueUploadFile? image, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<IssueDto>> GetMyIssuesAsync(string applicationUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<IssueTypeDto>> GetIssueTypesAsync(CancellationToken cancellationToken = default);
}
