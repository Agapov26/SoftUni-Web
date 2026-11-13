using System.ComponentModel.DataAnnotations;

namespace GrandStay.Models
{
    public class Guest
    {

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    }
}
