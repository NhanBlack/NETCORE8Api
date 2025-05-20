using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CleanSolution.Models;
using System.Security.Cryptography;

namespace CleanSolution.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginInput request)
        {
            // Giả lập user hợp lệ
            if (request.Username != "admin" || request.Password != "123456")
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, request.Username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this_is_a_very_long_secret_key_for_jwt_token_123456"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.UtcNow.AddHours(24);

            var token = new JwtSecurityToken(
                issuer: "your-issuer",
                audience: "your-audience",
                claims: claims,
                expires: expiry,
                signingCredentials: creds);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            // Tạo refresh token (random string)
            var refreshToken = GenerateRefreshToken();


            return Ok(new
            {
                token = "Bearer " + accessToken,
                refreshToken = refreshToken,
                expires = expiry
            });
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
