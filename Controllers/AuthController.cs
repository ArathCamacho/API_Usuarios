using API_Usuarios.Models;
using Microsoft.AspNetCore.Mvc;

namespace API_Usuarios.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginRequest>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);

                if (!response.Success)
                {
                    return Unauthorized(response);
                }

                // No devolver la contraseña hasheada en la respuesta
                response.Usuario.Password = null;

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(500, new LoginRequest
                {
                    Success = false,
                    Message = "Ha ocurrido un error interno al procesar su solicitud."
                });
            }
        }
    }
}
