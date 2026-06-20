using apiContact.Models.Entities;

namespace apiContact.Data.Seeds
{
    public static class UserSeed
    {
        public static IReadOnlyList<ChatUser> Generate()
        {
            var hash = BCrypt.Net.BCrypt.HashPassword("password123", workFactor: 12);

            return new List<ChatUser>
            {
                new()
                {
                    Id          = "user_001",
                    Username    = "alice",
                    DisplayName = "Alice Johnson",
                    Email       = "alice@chat.io",
                    Role        = "admin",
                    IsOnline    = true,
                    PasswordHash = hash
                },
                new()
                {
                    Id          = "user_002",
                    Username    = "bob",
                    DisplayName = "Bob Smith",
                    Email       = "bob@chat.io",
                    Role        = "user",
                    IsOnline    = false,
                    PasswordHash = hash
                },
                new()
                {
                    Id          = "user_003",
                    Username    = "carla",
                    DisplayName = "Carla Mendes",
                    Email       = "carla@chat.io",
                    Role        = "user",
                    IsOnline    = true,
                    PasswordHash = hash
                }
            };
        }
    }
}
