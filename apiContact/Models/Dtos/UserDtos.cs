namespace apiContact.Models.Dtos
{
    public class CreateUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
    }

    public class UpdateUserDto
    {
        public string? DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
        public bool? IsOnline { get; set; }
    }
}
