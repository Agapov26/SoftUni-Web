using System.ComponentModel.DataAnnotations;

namespace GrandStay.Models
{
    public class Room
    {

        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string RoomNumber { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        [Required]
        public int RoomTypeId { get; set; }

        public RoomType RoomType { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        public ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();

    }
}
