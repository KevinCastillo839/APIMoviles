using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;

namespace api.Services
{
    public class AuthService
    {
        private readonly IConfiguration _config;

        public AuthService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.full_name),
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                // Agrega otros claims necesarios
            };

            // Usamos _config en vez de builder.Configuration
            var key = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("Jwt:Key", "La clave JWT no está configurada en la configuración");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
       public string GenerateResetToken(User user)
        {
            var random = new Random();
            // Generar un token numérico de 6 dígitos
            int token = random.Next(100000, 999999); // Rango entre 100000 y 999999 (inclusive)
            return token.ToString();
        }


        public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

       
        public bool VerifyPassword(string password, string hashedPassword) => BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    private static readonly Dictionary<string, string> TempTokens = new Dictionary<string, string>();

    public void SaveToken(string email, string token)
    {
        TempTokens[email] = token;
    }

    public bool ValidateToken(string email, string token)
    {
        return TempTokens.TryGetValue(email, out string storedToken) && storedToken == token;
    }

    public void RemoveToken(string email)
    {
        TempTokens.Remove(email);
    }  
     
    }
}
