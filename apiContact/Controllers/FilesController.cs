using apiContact.Models.Dtos;
using apiContact.Models.Entities;
using apiContact.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using apiContact.Hubs;

namespace apiContact.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _files;
        private readonly IMessageService _messages;
        private readonly IHubContext<ChatHub> _hub;

        public FilesController(IFileService files, IMessageService messages, IHubContext<ChatHub> hub)
        {
            _files = files;
            _messages = messages;
            _hub = hub;
        }

        /// <summary>Upload a file and optionally attach it to a room message</summary>
        [HttpPost("upload")]
        [RequestSizeLimit(20_000_000)]
        public async Task<IActionResult> Upload(IFormFile file, [FromForm] string? roomId, [FromForm] string? senderId)
        {
            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse<object>.Fail("No file provided"));

            var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".pdf", ".txt", ".zip", ".mp4", ".mp3" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext))
                return BadRequest(ApiResponse<object>.Fail($"File type '{ext}' not allowed"));

            var (url, fileName, size) = await _files.UploadAsync(file);

            object result = new { url, fileName, size, ext };

            if (!string.IsNullOrWhiteSpace(roomId) && !string.IsNullOrWhiteSpace(senderId))
            {
                var isImage = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }.Contains(ext);
                var msg = await _messages.SendAsync(new SendMessageDto
                {
                    RoomId = roomId,
                    SenderId = senderId,
                    Content = fileName,
                    Type = isImage ? MessageType.Image : MessageType.File
                });
                msg.FileUrl = url;
                msg.FileName = fileName;
                msg.FileSize = size;

                await _hub.Clients.Group(roomId).SendAsync("ReceiveMessage", new
                {
                    msg.Id, msg.RoomId, msg.SenderId, msg.SenderName,
                    msg.Content, Type = msg.Type.ToString(),
                    msg.FileUrl, msg.FileName, msg.FileSize, msg.Timestamp
                });

                result = new { url, fileName, size, ext, messageId = msg.Id };
            }

            return Ok(ApiResponse<object>.Ok(result, "File uploaded"));
        }

        /// <summary>Delete an uploaded file by name</summary>
        [HttpDelete("{fileName}")]
        public async Task<IActionResult> Delete(string fileName)
        {
            var ok = await _files.DeleteAsync(fileName);
            if (!ok) return NotFound(ApiResponse<object>.Fail("File not found"));
            return Ok(ApiResponse<object>.Ok(new { fileName }, "File deleted"));
        }
    }
}
