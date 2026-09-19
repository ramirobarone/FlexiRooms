using Infrastructure.Models;

namespace Application.Models
{
    public class IssueDto(int Id, int? BookingId, int TipoDeReclamo, string TipoDeReclamoNombre, string Texto, string? Imagen, string Estado, DateTime CreatedAtUtc)
    {
        public int Id { get; } = Id;
        public int? BookingId { get; } = BookingId;
        public int TipoDeReclamo { get; } = TipoDeReclamo;
        public string TipoDeReclamoNombre { get; } = TipoDeReclamoNombre;
        public string Texto { get; } = Texto;
        public string? Imagen { get; } = Imagen;
        public string Estado { get; } = Estado;
        public DateTime CreatedAtUtc { get; } = CreatedAtUtc;

        public static implicit operator IssueDto(Issue issue)
        {
            return new IssueDto(issue.Id, issue.BookingId, issue.MaintenanceTypeId, issue.MaintenanceType?.Description ?? string.Empty, issue.Text, issue.Image, issue.Status, issue.CreatedAtUtc);
        }
    }
}
