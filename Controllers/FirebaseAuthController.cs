using Firebase.Authentication.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;

namespace Firebase.Authentication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FirebaseAuthController : ControllerBase
    {
        private readonly FirebaseHelper _firebaseHelper;

        public FirebaseAuthController(FirebaseHelper firebaseHelper)
        {
            _firebaseHelper = firebaseHelper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var uid = await _firebaseHelper.CreateUserAsync(request.Email, request.Password);
            return Ok(new { Uid = uid });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _firebaseHelper.SignInWithEmailAsync(request.Email, request.Password);
            return Ok(JsonDocumentToObject(result));
        }

        [HttpGet("userid")]
        public async Task<IActionResult> GetUserId([FromQuery] string token)
        {
            var uid = await _firebaseHelper.GetUserIdAsync(token);
            return Ok(new { Uid = uid });
        }

        [HttpDelete("{uid}")]
        public async Task<IActionResult> DeleteUser(string uid)
        {
            await _firebaseHelper.DeleteUserAsync(uid);
            return NoContent();
        }

        [HttpPut("{uid}")]
        public async Task<IActionResult> UpdateUser(string uid, [FromBody] UpdateUserRequest request)
        {
            await _firebaseHelper.UpdateUserAsync(uid, request.Email, request.Password);
            return NoContent();
        }

        [HttpGet("{uid}")]
        public async Task<IActionResult> GetUser(string uid)
        {
            var user = await _firebaseHelper.GetUserAsync(uid);
            return Ok(user);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> GenerateTokenFromRefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await _firebaseHelper.GenerateTokenFromRefreshTokenAsync(request.RefreshToken);
            return Ok(JsonDocumentToObject(result));
        }

        // Helper to convert JsonDocument to object for serialization
        private static object JsonDocumentToObject(JsonDocument doc)
        {
            return JsonSerializer.Deserialize<object>(doc.RootElement.GetRawText());
        }
    }

    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UpdateUserRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }
}