using GrandStay.Data;
using GrandStay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GrandStay.Controllers
{
    public class AmenitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AmenitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var amenities = await _context.Amenities
                .Include(a => a.RoomAmenities)
                .OrderBy(a => a.Name)
                .ToListAsync();

            return View(amenities);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var amenity = await _context.Amenities
                .Include(a => a.RoomAmenities)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (amenity == null)
            {
                return NotFound();
            }

            return View(amenity);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name,Description")] Amenity amenity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(amenity);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Amenity created successfully.";

                return RedirectToAction(nameof(Index));
            }

            return View(amenity);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var amenity = await _context.Amenities.FindAsync(id);

            if (amenity == null)
            {
                return NotFound();
            }

            return View(amenity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Name,Description")] Amenity amenity)
        {
            if (id != amenity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(amenity);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Amenity updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AmenityExists(amenity.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(amenity);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var amenity = await _context.Amenities
                .Include(a => a.RoomAmenities)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (amenity == null)
            {
                return NotFound();
            }

            return View(amenity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var amenity = await _context.Amenities
                .Include(a => a.RoomAmenities)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (amenity == null)
            {
                return NotFound();
            }

            if (amenity.RoomAmenities.Any())
            {
                TempData["ErrorMessage"] =
                    "This amenity cannot be deleted because it is assigned to existing rooms.";

                return RedirectToAction(nameof(Index));
            }

            _context.Amenities.Remove(amenity);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Amenity deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        private bool AmenityExists(int id)
        {
            return _context.Amenities.Any(e => e.Id == id);
        }
    }
}