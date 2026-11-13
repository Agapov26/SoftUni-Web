using System.ComponentModel.DataAnnotations;

namespace GrandStay.Models
{
    public class RoomType
    {

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Range(1, 100000)]
        public decimal BasePrice { get; set; }

        [Range(1, 10)]
        public int MaxOccupancy { get; set; }

        public ICollection<Room> Rooms { get; set; } = new List<Room>();

    }
}
