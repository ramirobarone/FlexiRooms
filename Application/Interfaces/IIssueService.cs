using Application.Models;

namespace Application.Interfaces;

public interface IIssueService
{
    Task<IssueDto> CreateIssueAsync(string applicationUserId, int bookingId, int tipoDeReclamo, string texto, IssueUploadFile? image, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<IssueDto>> GetMyIssuesAsync(string applicationUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<IssueDto>> GetHotelIssuesAsync(string applicationUserId, bool isSuperAdmin, CancellationToken cancellationToken = default);
    Task<IssueDto> UpdateHotelIssueStatusAsync(string applicationUserId, bool isSuperAdmin, int issueId, string status, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<IssueTypeDto>> GetIssueTypesAsync(CancellationToken cancellationToken = default);
}
