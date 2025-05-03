using API_Usuarios.Models;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new LoginResponse { Success = false, Message = "Datos inválidos" });
            }

            var jefe = await _context.JefesDepartamento
                .FirstOrDefaultAsync(j => j.Nombre == request.Nombre);

            if (jefe == null || !BC.Verify(request.NumeroTarjeta, jefe.NumeroTarjeta))
            {
                return Unauthorized(new LoginResponse { Success = false, Message = "Nombre o número de tarjeta incorrectos" });
            }

            var token = _jwtHelper.GenerateToken(jefe);

            return Ok(new LoginResponse
            {
                Success = true,
                Message = "Inicio de sesión exitoso",
                Token = token,
                Nombre = jefe.Nombre,
                JefeId = jefe.Id
            });
        }
    }
}
