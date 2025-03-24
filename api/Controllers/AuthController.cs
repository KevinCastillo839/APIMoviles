using System.Linq;
using Microsoft.AspNetCore.Mvc;
using api.Data;
using api.Models;
using api.Services;
using api.Dtos.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;


namespace api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
    private readonly ApplicationDBContext _context;
    private readonly AuthService _authService;

    private readonly IConfiguration _config;

    public AuthController(ApplicationDBContext context, AuthService authService, IConfiguration config)
    {
        _context = context;
        _authService = authService;
        _config = config;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] User user)
    {
        // Verificar si el email ya existe
        if (_context.Users.Any(u => u.email == user.email))
            return BadRequest(new { message = "El usuario ya existe" });

        // Validar que el campo full_name no esté vacío
        if (string.IsNullOrWhiteSpace(user.full_name))
            return BadRequest(new { message = "El campo full_name es obligatorio" });

        // Validar la contraseña (al menos 8 caracteres, una letra mayúscula, una minúscula, un número y un carácter especial)
        var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
        if (!passwordRegex.IsMatch(user.password))
        {
            return BadRequest(new { message = "La contraseña debe tener al menos 8 caracteres, una letra mayúscula, una letra minúscula, un número y un carácter especial" });
        }

        // Hash de la contraseña
        user.password = _authService.HashPassword(user.password);

        // El campo Id se gestiona automáticamente por la base de datos
        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok(new { message = "Usuario registrado exitosamente" });
    }




        // **Inicio de Sesión**
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModelDto login)
    {
            // Verificar si el usuario existe
        var user = _context.Users.FirstOrDefault(u => u.email == login.email);
        if (user == null)
        {
                return Unauthorized(new { message = "Usuario no encontrado" });
            }

            // Verificar la contraseña
            bool isPasswordValid = _authService.VerifyPassword(login.password, user.password);
            if (!isPasswordValid)
            {
                return Unauthorized(new { message = "Contraseña incorrecta" });
            }

            // Generar el token JWT
            var token = _authService.GenerateJwtToken(user);

            // Devolver el token en la respuesta
            return Ok(new { message = "Inicio de sesión exitoso", token = token });
        }
       
    [HttpPost("forgot-password")]
    public IActionResult ForgotPassword([FromBody] ResetPasswordDto request)
    {
        var user = _context.Users.FirstOrDefault(u => u.email == request.Email);
        if (user == null)
        {
            return BadRequest(new { message = "No se encontró un usuario con este correo" });
        }

        var token = _authService.GenerateResetToken(user);
        _authService.SaveToken(user.email, token);

        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("kfoods68@gmail.com", "fodr zkyy qsot flos"),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress("kfoods68@gmail.com", "KFoods"),
            Subject = "Restablecimiento de contraseña",
            Body = $"Tu código de restablecimiento es: {token}",
            IsBodyHtml = true,
        };
                // Marcar el correo como importante
        mailMessage.Headers.Add("X-Priority", "1");  // Prioridad alta
        mailMessage.Headers.Add("Importance", "High");  // Alta importancia
        mailMessage.To.Add(request.Email);

        smtpClient.Send(mailMessage);
        return Ok(new { message = "Correo de restablecimiento enviado exitosamente" });
    }
    [HttpPost("reset-password")]
    public IActionResult ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        if (!_authService.ValidateToken(request.Email, request.Token))
        {
            return BadRequest(new { message = "Token inválido o expirado" });
        }

        var user = _context.Users.FirstOrDefault(u => u.email == request.Email);
        if (user == null)
        {
            return BadRequest(new { message = "Usuario no encontrado" });
        }

        // Validar la nueva contraseña (al menos 8 caracteres, una letra mayúscula, una minúscula, un número y un carácter especial)
        var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
        if (!passwordRegex.IsMatch(request.NewPassword))
        {
            return BadRequest(new { message = "La nueva contraseña debe tener al menos 8 caracteres, una letra mayúscula, una letra minúscula, un número y un carácter especial" });
        }

        // Hash de la nueva contraseña
        user.password = _authService.HashPassword(request.NewPassword);
        _context.SaveChanges();

        _authService.RemoveToken(request.Email);

        return Ok(new { message = "Contraseña restablecida exitosamente" });
    }



    }

    


}
