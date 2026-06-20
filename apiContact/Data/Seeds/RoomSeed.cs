using apiContact.Models.Entities;

namespace apiContact.Data.Seeds
{
    public static class RoomSeed
    {
        public static IReadOnlyList<ChatRoom> Generate(IReadOnlyList<ChatUser> users)
        {
            var u1 = users[0];
            var u2 = users[1];
            var u3 = users[2];

            return new List<ChatRoom>
            {
                new()
                {
                    Id          = "room_001",
                    Name        = "General",
                    Description = "General discussion — open to all",
                    Type        = RoomType.Channel,
                    MemberIds   = new List<string> { u1.Id, u2.Id, u3.Id },
                    CreatedBy   = u1.Id
                },
                new()
                {
                    Id          = "room_002",
                    Name        = "Engineering",
                    Description = "Engineering team chat",
                    Type        = RoomType.Group,
                    MemberIds   = new List<string> { u1.Id, u3.Id },
                    CreatedBy   = u1.Id
                },
                new()
                {
                    Id          = "room_003",
                    Name        = "Alice ↔ Bob",
                    Description = "Direct message",
                    Type        = RoomType.Direct,
                    MemberIds   = new List<string> { u1.Id, u2.Id },
                    CreatedBy   = u1.Id
                }
            };
        }
    }
}
