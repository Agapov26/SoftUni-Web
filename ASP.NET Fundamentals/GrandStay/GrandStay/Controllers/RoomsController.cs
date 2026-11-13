using GrandStay.Data;
using GrandStay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GrandStay.Controllers
{
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var rooms = await _context.Rooms
                .Include(r => r.RoomType)
                .ToListAsync();

            return View(rooms);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateRoomTypesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("RoomNumber,Description,ImageUrl,RoomTypeId,IsActive")]
            Room room)
        {
            ModelState.Remove(nameof(Room.RoomType));

            if (ModelState.IsValid)
            {
                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Room created successfully.";

                return RedirectToAction(nameof(Index));
            }

            await PopulateRoomTypesAsync(room.RoomTypeId);
            return View(room);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            await PopulateRoomTypesAsync(room.RoomTypeId);
            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,RoomNumber,Description,ImageUrl,RoomTypeId,IsActive")]
            Room updatedRoom)
        {
            if (id != updatedRoom.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(Room.RoomType));

            if (!ModelState.IsValid)
            {
                await PopulateRoomTypesAsync(updatedRoom.RoomTypeId);
                return View(updatedRoom);
            }

            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            room.RoomNumber = updatedRoom.RoomNumber;
            room.Description = updatedRoom.Description;
            room.ImageUrl = updatedRoom.ImageUrl;
            room.RoomTypeId = updatedRoom.RoomTypeId;
            room.IsActive = updatedRoom.IsActive;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Room updated successfully.";

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Bookings)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            if (room.Bookings.Any())
            {
                TempData["ErrorMessage"] =
                    "This room cannot be deleted because it has existing bookings.";

                return RedirectToAction(nameof(Index));
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Room deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateRoomTypesAsync(
            int? selectedRoomType = null)
        {
            var roomTypes = await _context.RoomTypes
                .OrderBy(rt => rt.Name)
                .ToListAsync();

            ViewBag.RoomTypeId = new SelectList(
                roomTypes,
                "Id",
                "Name",
                selectedRoomType);
        }
    }
}