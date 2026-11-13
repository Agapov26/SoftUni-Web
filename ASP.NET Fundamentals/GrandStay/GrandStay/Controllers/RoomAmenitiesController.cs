using GrandStay.Data;
using GrandStay.Models;
using GrandStay.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GrandStay.Controllers
{
    public class RoomAmenitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomAmenitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Manage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.RoomAmenities)
                    .ThenInclude(ra => ra.Amenity)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            var availableAmenities = await _context.Amenities
                .OrderBy(a => a.Name)
                .ToListAsync();

            var viewModel = new RoomAmenitiesViewModel
            {
                Room = room,
                AvailableAmenities = availableAmenities,
                SelectedAmenityIds = room.RoomAmenities
                    .Select(ra => ra.AmenityId)
                    .ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(
            int id,
            List<int> selectedAmenityIds)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomAmenities)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            var validAmenityIds = await _context.Amenities
                .Where(a => selectedAmenityIds.Contains(a.Id))
                .Select(a => a.Id)
                .ToListAsync();

            room.RoomAmenities.Clear();

            foreach (var amenityId in validAmenityIds.Distinct())
            {
                room.RoomAmenities.Add(new RoomAmenity
                {
                    RoomId = room.Id,
                    AmenityId = amenityId
                });
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Room amenities updated successfully.";

            return RedirectToAction(nameof(Manage), new { id });
        }
    }
}