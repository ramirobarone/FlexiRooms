using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
    public class Room
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }
        public int BedNumbers { get; set; }
        public bool AvialableNow { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Hotel? Hotels { get; set; }
        public ICollection<Cost>? Costs { get; set; }
        public ICollection<RoomPicture>? RoomPictures { get; set; }
    }
}