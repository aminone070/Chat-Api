using apiContact.Models.Dtos;
using apiContact.Models.Entities;
using apiContact.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using apiContact.Hubs;

namespace apiContact.Controllers
{
    [ApiController]
    [Route("api/messages")]
    [Produces("application/json")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messages;
        private readonly IRoomService _rooms;
        private readonly IHubContext<ChatHub> _hub;

        public MessagesController(IMessageService messages, IRoomService rooms, IHubContext<ChatHub> hub)
        {
            _messages = messages;
            _rooms = rooms;
            _hub = hub;
        }

        /// <summary>Get messages for a room</summary>
        [HttpGet("room/{roomId}")]
        public async Task<IActionResult> GetByRoom(string roomId, [FromQuery] int limit = 50, [FromQuery] int skip = 0)
        {
            var list = await _messages.GetByRoomAsync(roomId, limit, skip);
            return Ok(ApiResponse<object>.Ok(list, total: list.Count));
        }

        /// <summary>Get a single message</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var msg = await _messages.GetByIdAsync(id);
            if (msg == null) return NotFound(ApiResponse<object>.Fail("Message not found"));
            return Ok(ApiResponse<object>.Ok(msg));
        }

        /// <summary>Send a message</summary>
        [HttpPost]
        public async Task<IActionResult> Send([FromBody] SendMessageDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest(ApiResponse<object>.Fail("Message content is required"));

            var msg = await _messages.SendAsync(dto);

            await _rooms.UpdateLastMessageAsync(dto.RoomId, dto.Content.Length > 60 ? dto.Content[..60] + "…" : dto.Content);

            await _hub.Clients.Group(dto.RoomId).SendAsync("ReceiveMessage", new
            {
                msg.Id, msg.RoomId, msg.SenderId, msg.SenderName,
                msg.Content, Type = msg.Type.ToString(), msg.Timestamp
            });

            return CreatedAtAction(nameof(GetById), new { id = msg.Id },
                ApiResponse<object>.Ok(msg, "Message sent"));
        }

        /// <summary>Edit a message</summary>
        [HttpPatch("{id}")]
        public async Task<IActionResult> Edit(string id, [FromBody] EditMessageDto dto)
        {
            var msg = await _messages.GetByIdAsync(id);
            if (msg == null) return NotFound(ApiResponse<object>.Fail("Message not found"));

            var updated = await _messages.EditAsync(id, msg.SenderId, dto.Content);

            await _hub.Clients.Group(msg.RoomId).SendAsync("MessageEdited", new { id, content = dto.Content });

            return Ok(ApiResponse<object>.Ok(updated, "Message edited"));
        }

        /// <summary>Delete a message</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var msg = await _messages.GetByIdAsync(id);
            if (msg == null) return NotFound(ApiResponse<object>.Fail("Message not found"));

            await _messages.DeleteAsync(id, msg.SenderId);

            await _hub.Clients.Group(msg.RoomId).SendAsync("MessageDeleted", new { id, roomId = msg.RoomId });

            return Ok(ApiResponse<object>.Ok(new { id }, "Message deleted"));
        }

        /// <summary>Mark a message as read</summary>
        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkRead(string id, [FromBody] MarkReadDto dto)
        {
            await _messages.MarkReadAsync(id, dto.UserId);
            return Ok(ApiResponse<object>.Ok(new { id, userId = dto.UserId }, "Marked as read"));
        }

        /// <summary>Get unread count for a room</summary>
        [HttpGet("room/{roomId}/unread")]
        public async Task<IActionResult> UnreadCount(string roomId, [FromQuery] string userId)
        {
            var count = await _messages.GetUnreadCountAsync(roomId, userId);
            return Ok(ApiResponse<object>.Ok(new { roomId, userId, unreadCount = count }));
        }
    }
}
