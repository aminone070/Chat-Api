using apiContact.Data;
using apiContact.Models.Dtos;
using apiContact.Models.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace apiContact.Services
{
    public class RoomService : IRoomService
    {
        private readonly ChatDbContext _db;
        private readonly IMongoCollection<ChatRoom>? _col;

        public RoomService(ChatDbContext db)
        {
            _db = db;
            _col = db.GetCollection<ChatRoom>("rooms");
        }

        public async Task<List<ChatRoom>> GetAllAsync()
        {
            if (_db.IsInMemory) return _db.Rooms.Values.OrderByDescending(r => r.LastMessageAt).ToList();
            return await _col!.Find(_ => true).SortByDescending(r => r.LastMessageAt).ToListAsync();
        }

        public async Task<List<ChatRoom>> GetByUserAsync(string userId)
        {
            if (_db.IsInMemory) return _db.Rooms.Values.Where(r => r.MemberIds.Contains(userId)).OrderByDescending(r => r.LastMessageAt).ToList();
            return await _col!.Find(r => r.MemberIds.Contains(userId)).SortByDescending(r => r.LastMessageAt).ToListAsync();
        }

        public async Task<ChatRoom?> GetByIdAsync(string id)
        {
            if (_db.IsInMemory) return _db.Rooms.GetValueOrDefault(id);
            return await _col!.Find(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task<ChatRoom> CreateAsync(CreateRoomDto dto)
        {
            var room = new ChatRoom
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = dto.Name,
                Description = dto.Description,
                Type = dto.Type,
                MemberIds = dto.MemberIds,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };
            if (_db.IsInMemory) { _db.Rooms[room.Id] = room; return room; }
            await _col!.InsertOneAsync(room);
            return room;
        }

        public async Task<bool> AddMemberAsync(string roomId, string userId)
        {
            var room = await GetByIdAsync(roomId);
            if (room == null || room.MemberIds.Contains(userId)) return false;
            room.MemberIds.Add(userId);
            if (_db.IsInMemory) { _db.Rooms[roomId] = room; return true; }
            await _col!.UpdateOneAsync(r => r.Id == roomId, Builders<ChatRoom>.Update.AddToSet(r => r.MemberIds, userId));
            return true;
        }

        public async Task<bool> RemoveMemberAsync(string roomId, string userId)
        {
            var room = await GetByIdAsync(roomId);
            if (room == null) return false;
            room.MemberIds.Remove(userId);
            if (_db.IsInMemory) { _db.Rooms[roomId] = room; return true; }
            await _col!.UpdateOneAsync(r => r.Id == roomId, Builders<ChatRoom>.Update.Pull(r => r.MemberIds, userId));
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            if (_db.IsInMemory) return _db.Rooms.Remove(id);
            var result = await _col!.DeleteOneAsync(r => r.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task UpdateLastMessageAsync(string roomId, string preview)
        {
            var room = await GetByIdAsync(roomId);
            if (room == null) return;
            room.LastMessagePreview = preview;
            room.LastMessageAt = DateTime.UtcNow;
            if (_db.IsInMemory) { _db.Rooms[roomId] = room; return; }
            await _col!.UpdateOneAsync(r => r.Id == roomId,
                Builders<ChatRoom>.Update
                    .Set(r => r.LastMessagePreview, preview)
                    .Set(r => r.LastMessageAt, DateTime.UtcNow));
        }
    }
}
