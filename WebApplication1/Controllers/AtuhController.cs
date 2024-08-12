using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WEBAPI.Data;
using WEBAPI.DTOs;
using WEBAPI.Models;

namespace WEBAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly prueba_tecnicaContext _dbContext;
        private readonly JwtSettings _jwtSettings;

        public AuthController(prueba_tecnicaContext dbContext, IOptions<JwtSettings> jwtSettings)
        {
            _dbContext = dbContext;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] UserLoginDTO userLogin)
        {
            var user = _dbContext.Users
                .FirstOrDefault(u => u.UserEmail == userLogin.UserEmail && u.UserPassword == userLogin.Password);

            if (user != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                // Decodificar la clave Base64 a un array de bytes
                var keyBytes = Convert.FromBase64String(_jwtSettings.Key);

                // Verificar la longitud de la clave
                if (keyBytes.Length * 8 != 256)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Invalid key size." });
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.UserEmail),
                new Claim("UserId", user.UserId.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiresInHours),
                    Issuer = _jwtSettings.Issuer,  // Configurar el emisor
                    Audience = _jwtSettings.Audience,  // Configurar el receptor
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Ok(new { token = tokenString });
            }

            return Unauthorized("Invalid credentials");
        }

        public static string GenerateSecretKey(int keySize = 256)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var key = new byte[keySize / 8];
                rng.GetBytes(key);
                return Convert.ToBase64String(key);
            }
        }

    }
}
