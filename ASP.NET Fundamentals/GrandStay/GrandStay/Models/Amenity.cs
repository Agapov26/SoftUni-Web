using System.ComponentModel.DataAnnotations;

namespace GrandStay.Models
{
    public class Amenity
    {

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [StringLength(250)]
        public string Description { get; set; } = string.Empty;

        public ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();

    }
}
