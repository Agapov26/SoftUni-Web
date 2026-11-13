using GrandStay.Data;
using GrandStay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GrandStay.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Room)
                    .ThenInclude(r => r.RoomType)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return View(bookings);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Room)
                    .ThenInclude(r => r.RoomType)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("GuestId,RoomId,CheckIn,CheckOut")]
            Booking booking)
        {
            ModelState.Remove(nameof(Booking.TotalPrice));

            if (booking.CheckOut.Date <= booking.CheckIn.Date)
            {
                ModelState.AddModelError(
                    nameof(Booking.CheckOut),
                    "Check-out date must be after check-in date.");
            }

            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.Id == booking.RoomId);

            if (room == null)
            {
                ModelState.AddModelError(
                    nameof(Booking.RoomId),
                    "Please select a valid room.");
            }
            else if (!room.IsActive)
            {
                ModelState.AddModelError(
                    nameof(Booking.RoomId),
                    "This room is currently inactive.");
            }

            var guestExists = await _context.Guests
                .AnyAsync(g => g.Id == booking.GuestId);

            if (!guestExists)
            {
                ModelState.AddModelError(
                    nameof(Booking.GuestId),
                    "Please select a valid guest.");
            }

            if (room != null &&
                booking.CheckOut.Date > booking.CheckIn.Date)
            {
                bool hasOverlap = await _context.Bookings.AnyAsync(b =>
                    b.RoomId == booking.RoomId &&
                    b.Status != BookingStatus.Cancelled &&
                    b.CheckIn < booking.CheckOut &&
                    b.CheckOut > booking.CheckIn);

                if (hasOverlap)
                {
                    ModelState.AddModelError(
                        nameof(Booking.RoomId),
                        "This room is already booked for the selected dates.");
                }

                int nights = (booking.CheckOut.Date -
                              booking.CheckIn.Date).Days;

                booking.TotalPrice = room.RoomType.BasePrice * nights;
            }

            if (ModelState.IsValid)
            {
                booking.Status = BookingStatus.Pending;
                booking.CreatedAt = DateTime.UtcNow;

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Booking created successfully.";

                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdownsAsync(
                booking.GuestId,
                booking.RoomId,
                booking.Status);

            return View(booking);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            await PopulateDropdownsAsync(
                booking.GuestId,
                booking.RoomId,
                booking.Status);

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,GuestId,RoomId,CheckIn,CheckOut,Status")]
            Booking updatedBooking)
        {
            if (id != updatedBooking.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(Booking.TotalPrice));

            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            if (updatedBooking.CheckOut.Date <=
                updatedBooking.CheckIn.Date)
            {
                ModelState.AddModelError(
                    nameof(Booking.CheckOut),
                    "Check-out date must be after check-in date.");
            }

            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.Id == updatedBooking.RoomId);

            if (room == null)
            {
                ModelState.AddModelError(
                    nameof(Booking.RoomId),
                    "Please select a valid room.");
            }
            else if (!room.IsActive)
            {
                ModelState.AddModelError(
                    nameof(Booking.RoomId),
                    "This room is currently inactive.");
            }

            bool guestExists = await _context.Guests
                .AnyAsync(g => g.Id == updatedBooking.GuestId);

            if (!guestExists)
            {
                ModelState.AddModelError(
                    nameof(Booking.GuestId),
                    "Please select a valid guest.");
            }

            if (room != null &&
                updatedBooking.CheckOut.Date >
                updatedBooking.CheckIn.Date)
            {
                bool hasOverlap = await _context.Bookings.AnyAsync(b =>
                    b.Id != id &&
                    b.RoomId == updatedBooking.RoomId &&
                    b.Status != BookingStatus.Cancelled &&
                    b.CheckIn < updatedBooking.CheckOut &&
                    b.CheckOut > updatedBooking.CheckIn);

                if (hasOverlap)
                {
                    ModelState.AddModelError(
                        nameof(Booking.RoomId),
                        "This room is already booked for the selected dates.");
                }

                int nights = (updatedBooking.CheckOut.Date -
                              updatedBooking.CheckIn.Date).Days;

                updatedBooking.TotalPrice =
                    room.RoomType.BasePrice * nights;
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(
                    updatedBooking.GuestId,
                    updatedBooking.RoomId,
                    updatedBooking.Status);

                return View(updatedBooking);
            }

            booking.GuestId = updatedBooking.GuestId;
            booking.RoomId = updatedBooking.RoomId;
            booking.CheckIn = updatedBooking.CheckIn;
            booking.CheckOut = updatedBooking.CheckOut;
            booking.TotalPrice = updatedBooking.TotalPrice;
            booking.Status = updatedBooking.Status;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Booking updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Booking deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdownsAsync(
            int? selectedGuest = null,
            int? selectedRoom = null,
            BookingStatus selectedStatus = BookingStatus.Pending)
        {
            var guests = await _context.Guests
                .OrderBy(g => g.LastName)
                .ThenBy(g => g.FirstName)
                .ToListAsync();

            var rooms = await _context.Rooms
                .Where(r => r.IsActive)
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();

            ViewBag.GuestId = new SelectList(
                guests,
                "Id",
                "Email",
                selectedGuest);

            ViewBag.RoomId = new SelectList(
                rooms,
                "Id",
                "RoomNumber",
                selectedRoom);

            ViewBag.Status = new SelectList(
                Enum.GetValues<BookingStatus>()
                    .Select(s => new
                    {
                        Id = s,
                        Name = s.ToString()
                    }),
                "Id",
                "Name",
                selectedStatus);
        }
    }
}