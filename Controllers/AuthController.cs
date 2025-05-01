using API_Usuarios.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using API_Usuarios.Data;
using API_Usuarios.Helpers;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace API_Usuarios.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public AuthController(ApplicationDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest loginRequest)
        {
            // Validar el modelo
            if (!ModelState.IsValid)
            {
                return BadRequest(new LoginResponse
                {
                    Success = false,
                    Message = "Datos de inicio de sesión inválidos"
                });
            }

            try
            {
                // Buscar el usuario por nombre de usuario o email
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u =>
                        u.Username == loginRequest.UsernameOrEmail ||
                        u.Email == loginRequest.UsernameOrEmail);

                // Verificar si el usuario existe
                if (usuario == null)
                {
                    return Unauthorized(new LoginResponse
                    {
                        Success = false,
                        Message = "Usuario o contraseña incorrectos"
                    });
                }

                // Verificar la contraseña
                if (!BC.Verify(loginRequest.Password, usuario.Password))
                {
                    return Unauthorized(new LoginResponse
                    {
                        Success = false,
                        Message = "Usuario o contraseña incorrectos"
                    });
                }

                // Generar el token JWT
                var token = _jwtHelper.GenerateToken(usuario);

                // Devolver la respuesta exitosa
                return Ok(new LoginResponse
                {
                    Success = true,
                    Token = token,
                    Username = usuario.Username,
                    UserId = usuario.Id,
                    Message = "Inicio de sesión exitoso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new LoginResponse
                {
                    Success = false,
                    Message = "Error al procesar la solicitud: " + ex.Message
                });
            }
        }
    }
}
