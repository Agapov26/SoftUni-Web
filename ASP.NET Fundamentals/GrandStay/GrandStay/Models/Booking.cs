using System.ComponentModel.DataAnnotations;

namespace GrandStay.Models
{
    public class Booking
    {

        public int Id { get; set; }

        [Required]
        public int GuestId { get; set; }

        public Guest Guest { get; set; } = null!;

        [Required]
        public int RoomId { get; set; }

        public Room Room { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckIn { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckOut { get; set; }

        [Range(0.01, 1000000)]
        public decimal TotalPrice { get; set; }

        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Completed
    }

}

