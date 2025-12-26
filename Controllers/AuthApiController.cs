using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartWeather.Services;
using SmartWeather.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartWeather.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthApiController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IConfiguration _config;

        public AuthApiController(UserService userService, IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }

        // =========================
        // POST: api/auth/register
        // =========================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Thiếu username hoặc password");
            }

            var existingUser = await _userService.GetByUsername(dto.Username);
            if (existingUser != null)
            {
                return BadRequest("Username đã tồn tại");
            }

            await _userService.Create(new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = dto.Password
            });

            return Ok(new
            {
                message = "Đăng ký thành công",
                username = dto.Username
            });
        }

        // =========================
        // POST: api/auth/login (JWT)
        // =========================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Thiếu username hoặc password");
            }

            var user = await _userService.GetByUsername(dto.Username);
            if (user == null ||
                !_userService.VerifyPassword(dto.Password, user.PasswordHash))
            {
                return Unauthorized("Sai username hoặc password");
            }

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                message = "Đăng nhập thành công",
                token = token
            });
        }

        // =========================
        // JWT GENERATOR
        // =========================
        private string GenerateJwtToken(User user)
        {
            var jwt = _config.GetSection("Jwt");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt["Key"]!)
            );

            var creds = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    double.Parse(jwt["ExpireMinutes"]!)
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // =========================
    // DTO
    // =========================
    public class RegisterDto
    {
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class LoginDto
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
