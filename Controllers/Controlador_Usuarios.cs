using API_Usuarios.Models;
using Microsoft.AspNetCore.Mvc;

namespace API_Usuarios.Controllers
{
    public class Controlador_Usuarios : ControllerBase
    {
        //Contiene los endpoints de la API (por ejemplo: /usuarios, /auth/login).
        //	Controlador con las rutas para crear, editar, borrar, y listar usuarios.
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioResponseDto>>> GetUsuarios([FromQuery] string filtro = null)
        {
            try
            {
                var usuarios = await _usuarioService.GetAllUsuariosAsync(filtro);
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor", Error = ex.Message });
            }
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioResponseDto>> GetUsuario(int id)
        {
            try
            {
                var usuario = await _usuarioService.GetUsuarioByIdAsync(id);

                if (usuario == null)
                {
                    return NotFound(new { Mensaje = $"Usuario con ID {id} no encontrado" });
                }

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor", Error = ex.Message });
            }
        }

        // POST: api/Usuarios
        [HttpPost]
        public async Task<ActionResult<UsuarioResponseDto>> CreateUsuario([FromBody] CreateUsuarioDto usuarioDto)
        {
            try
            {
                var usuario = await _usuarioService.CreateUsuarioAsync(usuarioDto);
                return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(int id, [FromBody] UpdateUsuarioDto usuarioDto)
        {
            try
            {
                var usuario = await _usuarioService.UpdateUsuarioAsync(id, usuarioDto);
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("no encontrado"))
                {
                    return NotFound(new { Mensaje = ex.Message });
                }
                return BadRequest(new { Mensaje = ex.Message });
            }
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            try
            {
                await _usuarioService.DeleteUsuarioAsync(id);
                return Ok(new { Mensaje = $"Usuario con ID {id} eliminado correctamente" });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("no encontrado"))
                {
                    return NotFound(new { Mensaje = ex.Message });
                }
                return StatusCode(500, new { Mensaje = "Error interno del servidor", Error = ex.Message });
            }
        }
    }
}
