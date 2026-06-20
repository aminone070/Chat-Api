using apiContact.Models.Dtos;
using apiContact.Services;
using Microsoft.AspNetCore.Mvc;

namespace apiContact.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _users;

        public UsersController(IUserService users) => _users = users;

        /// <summary>Get all users</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _users.GetAllAsync();
            return Ok(ApiResponse<object>.Ok(list, total: list.Count));
        }

        /// <summary>Get user by ID</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _users.GetByIdAsync(id);
            if (user == null) return NotFound(ApiResponse<object>.Fail("User not found"));
            return Ok(ApiResponse<object>.Ok(user));
        }

        /// <summary>Get user by username</summary>
        [HttpGet("by-username/{username}")]
        public async Task<IActionResult> GetByUsername(string username)
        {
            var user = await _users.GetByUsernameAsync(username);
            if (user == null) return NotFound(ApiResponse<object>.Fail("User not found"));
            return Ok(ApiResponse<object>.Ok(user));
        }

        /// <summary>Create a new user</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username))
                return BadRequest(ApiResponse<object>.Fail("Username is required"));

            var existing = await _users.GetByUsernameAsync(dto.Username);
            if (existing != null)
                return Conflict(ApiResponse<object>.Fail("Username already taken"));

            var user = await _users.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = user.Id },
                ApiResponse<object>.Ok(user, "User created"));
        }

        /// <summary>Update user profile</summary>
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto dto)
        {
            var user = await _users.UpdateAsync(id, dto);
            if (user == null) return NotFound(ApiResponse<object>.Fail("User not found"));
            return Ok(ApiResponse<object>.Ok(user, "User updated"));
        }

        /// <summary>Set user online status</summary>
        [HttpPost("{id}/status")]
        public async Task<IActionResult> SetStatus(string id, [FromBody] UpdateUserDto dto)
        {
            await _users.SetOnlineAsync(id, dto.IsOnline ?? false);
            return Ok(ApiResponse<object>.Ok(new { id, isOnline = dto.IsOnline }, "Status updated"));
        }

        /// <summary>Delete a user</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var ok = await _users.DeleteAsync(id);
            if (!ok) return NotFound(ApiResponse<object>.Fail("User not found"));
            return Ok(ApiResponse<object>.Ok(new { id }, "User deleted"));
        }
    }
}
