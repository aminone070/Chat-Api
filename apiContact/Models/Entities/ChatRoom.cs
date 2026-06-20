using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace apiContact.Models.Entities
{
    public enum RoomType { Direct, Group, Channel }

    public class ChatRoom
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public RoomType Type { get; set; } = RoomType.Group;
        public List<string> MemberIds { get; set; } = new();
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? LastMessagePreview { get; set; }
        public DateTime? LastMessageAt { get; set; }
    }
}
