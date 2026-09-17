using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
    public class IssueType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string Issue { get; set; }
    }
}
