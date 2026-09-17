using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
    public class Issue
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TipoDeReclamo { get; set; }
        public required string Texto { get; set; }
        public string? Imagen { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public required string ApplicationUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        public IssueType IssueType { get; set; } = null!;
        public ApplicationUser ApplicationUser { get; set; } = null!;
    }
}
