using apiContact.Models.Entities;
using MongoDB.Driver;

namespace apiContact.Data
{
    public class ChatDbContext
    {
        private readonly IMongoDatabase? _database;
        private readonly bool _useInMemory;

        // In-memory fallback stores
        internal readonly Dictionary<string, ChatUser> Users = new();
        internal readonly Dictionary<string, ChatRoom> Rooms = new();
        internal readonly Dictionary<string, Message> Messages = new();

        public ChatDbContext(IConfiguration config)
        {
            var connStr = config["MongoDB:ConnectionString"];
            var dbName = config["MongoDB:DatabaseName"] ?? "ChatDb";

            if (!string.IsNullOrWhiteSpace(connStr))
            {
                try
                {
                    var client = new MongoClient(connStr);
                    _database = client.GetDatabase(dbName);
                    _useInMemory = false;
                }
                catch
                {
                    _useInMemory = true;
                }
            }
            else
            {
                _useInMemory = true;
            }

            if (_useInMemory) SeedInMemory();
        }

        public bool IsInMemory => _useInMemory;

        public IMongoCollection<T>? GetCollection<T>(string name)
            => _database?.GetCollection<T>(name);

        private void SeedInMemory()
        {
            var u1 = new ChatUser { Id = "user_001", Username = "alice", DisplayName = "Alice Johnson", Email = "alice@chat.io", IsOnline = true };
            var u2 = new ChatUser { Id = "user_002", Username = "bob", DisplayName = "Bob Smith", Email = "bob@chat.io", IsOnline = false };
            var u3 = new ChatUser { Id = "user_003", Username = "carla", DisplayName = "Carla Mendes", Email = "carla@chat.io", IsOnline = true };
            Users[u1.Id] = u1; Users[u2.Id] = u2; Users[u3.Id] = u3;

            var r1 = new ChatRoom { Id = "room_001", Name = "General", Description = "General discussion", Type = RoomType.Channel, MemberIds = new() { u1.Id, u2.Id, u3.Id }, CreatedBy = u1.Id };
            var r2 = new ChatRoom { Id = "room_002", Name = "Engineering", Description = "Engineering team", Type = RoomType.Group, MemberIds = new() { u1.Id, u3.Id }, CreatedBy = u1.Id };
            Rooms[r1.Id] = r1; Rooms[r2.Id] = r2;

            var m1 = new Message { Id = "msg_001", RoomId = r1.Id, SenderId = u1.Id, SenderName = u1.DisplayName, Content = "Welcome to Chat API! 👋", Timestamp = DateTime.UtcNow.AddMinutes(-10) };
            var m2 = new Message { Id = "msg_002", RoomId = r1.Id, SenderId = u2.Id, SenderName = u2.DisplayName, Content = "Hey everyone!", Timestamp = DateTime.UtcNow.AddMinutes(-8) };
            var m3 = new Message { Id = "msg_003", RoomId = r1.Id, SenderId = u3.Id, SenderName = u3.DisplayName, Content = "This API is looking great.", Timestamp = DateTime.UtcNow.AddMinutes(-5) };
            Messages[m1.Id] = m1; Messages[m2.Id] = m2; Messages[m3.Id] = m3;

            r1.LastMessagePreview = m3.Content;
            r1.LastMessageAt = m3.Timestamp;
        }
    }
}
