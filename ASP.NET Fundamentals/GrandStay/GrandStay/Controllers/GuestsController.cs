using GrandStay.Data;
using GrandStay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GrandStay.Controllers
{
    public class GuestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GuestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var guests = await _context.Guests
                .Include(g => g.Bookings)
                .OrderBy(g => g.LastName)
                .ThenBy(g => g.FirstName)
                .ToListAsync();

            return View(guests);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guest = await _context.Guests
                .Include(g => g.Bookings)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (guest == null)
            {
                return NotFound();
            }

            return View(guest);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("FirstName,LastName,Email,PhoneNumber")] Guest guest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(guest);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Guest created successfully.";

                return RedirectToAction(nameof(Index));
            }

            return View(guest);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guest = await _context.Guests.FindAsync(id);

            if (guest == null)
            {
                return NotFound();
            }

            return View(guest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,FirstName,LastName,Email,PhoneNumber")] Guest guest)
        {
            if (id != guest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(guest);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Guest updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GuestExists(guest.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(guest);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guest = await _context.Guests
                .Include(g => g.Bookings)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (guest == null)
            {
                return NotFound();
            }

            return View(guest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var guest = await _context.Guests
                .Include(g => g.Bookings)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (guest == null)
            {
                return NotFound();
            }

            if (guest.Bookings.Any())
            {
                TempData["ErrorMessage"] =
                    "This guest cannot be deleted because they have existing bookings.";

                return RedirectToAction(nameof(Index));
            }

            _context.Guests.Remove(guest);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Guest deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        private bool GuestExists(int id)
        {
            return _context.Guests.Any(e => e.Id == id);
        }
    }
}