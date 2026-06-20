using apiContact.Models.Entities;

namespace apiContact.Models.Dtos
{
    public class SendMessageDto
    {
        public string RoomId { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; } = MessageType.Text;
    }

    public class EditMessageDto
    {
        public string Content { get; set; } = string.Empty;
    }

    public class MarkReadDto
    {
        public string UserId { get; set; } = string.Empty;
    }
}
