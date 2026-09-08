using MassageShop.API.Data;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MassageShop.API.Services
{
    public class RoomService : IRoomService
    {
        private readonly AppDbContext _db;

        public RoomService(AppDbContext db) => _db = db;

        public async Task<List<Room>> GetAllAsync() =>
            await _db.Rooms.ToListAsync();

        public async Task<Room?> GetByIdAsync(int id) =>
            await _db.Rooms.FindAsync(id);

        public async Task<Room> CreateAsync(Room room)
        {
            _db.Rooms.Add(room);
            await _db.SaveChangesAsync();
            return room;
        }

        public async Task<Room?> UpdateAsync(int id, Room updated)
        {
            var room = await _db.Rooms.FindAsync(id);
            if (room == null) return null;

            room.RoomNumber = updated.RoomNumber;
            room.RoomType = updated.RoomType;
            room.Capacity = updated.Capacity;
            room.Status = updated.Status;

            await _db.SaveChangesAsync();
            return room;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var room = await _db.Rooms.FindAsync(id);
            if (room == null) return false;
            _db.Rooms.Remove(room);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
