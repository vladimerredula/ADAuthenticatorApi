using ADAuthenticatorApi.Helpers;
using ADAuthenticatorApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ADAuthenticatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LdapAuthService _ldapService;
        private readonly JwtTokenService _jwtService;
        private readonly EncryptionHelper _aesHelper;

        public AuthController(LdapAuthService ldapService, JwtTokenService jwtService, EncryptionHelper encryption)
        {
            _ldapService = ldapService;
            _jwtService = jwtService;
            _aesHelper = encryption;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            string decryptedUsername, decryptedPassword;

            try
            {
                decryptedUsername = _aesHelper.Decrypt(request.Username);
                decryptedPassword = _aesHelper.Decrypt(request.Password);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to Decrypt: {ex.Message}");
                return BadRequest("Invalid encrypted payload.");
            }

            var (success, attributes) = _ldapService.AuthenticateAndGetAttributes(decryptedUsername, decryptedPassword);
            if (!success)
                return Unauthorized(new { Message = "Invalid credentials" });

            var token = _jwtService.GenerateToken(request.Username, attributes);

            return Ok(new { Token = token });
        }

        [HttpGet("protected")]
        [Authorize]
        public IActionResult Protected()
        {
            return Ok(new { Message = $"Hello {User.Identity?.Name}, you're authenticated!" });
        }

        public class LoginRequest
        {
            public string Username { get; set; } = "";
            public string Password { get; set; } = "";
        }
    }
}
