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
using api.Constants;


namespace api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
    private readonly ApplicationDBContext _context;
    private readonly AuthService _authService;

    private readonly IConfiguration _config;

    private readonly EmailService _emailService;

    public AuthController(ApplicationDBContext context, AuthService authService, IConfiguration config, EmailService emailService)
    {
        _context = context;
        _authService = authService;
        _config = config;
        _emailService = emailService;
    }


[HttpPost("register")]
public async Task<IActionResult> Register([FromBody] User user)
{
    if (_context.Users.Any(u => u.email == user.email))
        return BadRequest(new { message = "El usuario ya existe" });

    if (string.IsNullOrWhiteSpace(user.full_name))
        return BadRequest(new { message = "El campo full_name es obligatorio" });

    var passwordRegex = new Regex(RegexConstants.PasswordPattern);
    if (!passwordRegex.IsMatch(user.password))
    {
        return BadRequest(new { message = "La contraseña debe tener al menos 8 caracteres, una letra mayúscula, una letra minúscula, un número y un carácter especial" });
    }

    user.password = _authService.HashPassword(user.password);
    _context.Users.Add(user);
    _context.SaveChanges();

    await _emailService.SendVerificationEmailAsync(user.email);

<<<<<<< Updated upstream
    return Ok(new { message = "Usuario registrado exitosamente" });
=======
    var token = _authService.GenerateJwtToken(user);

    return Ok(new
    {
        message = "Usuario registrado exitosamente",
        userId = user.id,
        token = token
    });
}

[HttpGet("user/{id}")]
public IActionResult GetUserById(int id)
{
    var user = _context.Users.FirstOrDefault(u => u.id == id);
    if (user == null)
    {
        return NotFound(new { message = "Usuario no encontrado" });
    }

    return Ok(new
    {
        id = user.id,
        email = user.email,
        full_name = user.full_name,
        created_at = user.created_at
    });
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
            // Verificar la contraseña
            bool isPasswordValid = _authService.VerifyPassword(login.password, user.password);
            if (!isPasswordValid)
            {
                return Unauthorized(new { message = "Contraseña incorrecta" });
            }
=======
    bool isPasswordValid = _authService.VerifyPassword(login.password, user.password);
    if (!isPasswordValid)
    {
        return Unauthorized(new { message = "Contraseña incorrecta" });
    }

    var token = _authService.GenerateJwtToken(user);

    return Ok(new 
    { 
        message = "Inicio de sesión exitoso", 
        token = token,
        userId = user.id 
    });
}
[HttpDelete("delete-account/{id}")]
public IActionResult DeleteAccount(int id)
{
    var user = _context.Users.FirstOrDefault(u => u.id == id);
    if (user == null)
    {
        return NotFound(new { message = "Usuario no encontrado" });
    }

    _context.Users.Remove(user);
    _context.SaveChanges();

    return Ok(new { message = "Cuenta eliminada exitosamente" });
}
[HttpPost("logout")]
public IActionResult Logout()
{
  
    return Ok(new { message = "Sesión cerrada exitosamente" });
}
>>>>>>> Stashed changes

            // Generar el token JWT
            var token = _authService.GenerateJwtToken(user);

            // Devolver el token en la respuesta
            return Ok(new { message = "Inicio de sesión exitoso", token = token });
        }
       
  [HttpPost("forgot-password")]
public async Task<IActionResult> ForgotPassword([FromBody] ResetPasswordDto request)
{
    var user = _context.Users.FirstOrDefault(u => u.email == request.Email);
    if (user == null)
    {
        return BadRequest(new { message = "No se encontró un usuario con este correo" });
    }

    var token = _authService.GenerateResetToken(user);
    _authService.SaveToken(user.email, token);
    
    await _emailService.SendPasswordResetEmailAsync(request.Email, token);

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

        var passwordRegex = new Regex(RegexConstants.PasswordPattern);
        if (!passwordRegex.IsMatch(request.NewPassword))
        {
            return BadRequest(new { message = "La contraseña debe tener al menos 8 caracteres, una letra mayúscula, una letra minúscula, un número y un carácter especial" });
        }

        user.password = _authService.HashPassword(request.NewPassword);
        _context.SaveChanges();

        _authService.RemoveToken(request.Email);

        return Ok(new { message = "Contraseña restablecida exitosamente" });
    }



    }

    


}
