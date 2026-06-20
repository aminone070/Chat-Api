using apiContact.Models.Entities;

namespace apiContact.Models.Dtos
{
    public class CreateRoomDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public RoomType Type { get; set; } = RoomType.Group;
        public List<string> MemberIds { get; set; } = new();
        public string CreatedBy { get; set; } = string.Empty;
    }

    public class AddMemberDto
    {
        public string UserId { get; set; } = string.Empty;
    }
}
