using GrandStay.Models;

namespace GrandStay.ViewModels
{
    public class RoomAmenitiesViewModel
    {
        public Room Room { get; set; } = null!;

        public List<Amenity> AvailableAmenities { get; set; } = new();

        public List<int> SelectedAmenityIds { get; set; } = new();
    }
}