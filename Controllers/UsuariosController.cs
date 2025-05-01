using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_Usuarios.Data;
using API_Usuarios.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace API_Usuarios.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        //Contiene los endpoints de la API (por ejemplo: /usuarios, /auth/login).
        //	Controlador con las rutas para crear, editar, borrar, y listar usuarios.
        private readonly ApplicationDbContext   _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios(
            [FromQuery] string buscar = "",
            [FromQuery] bool? activo = null)
        {
            var query = _context.Usuarios.AsQueryable();

            // Aplicar filtros si existen
            if (!string.IsNullOrEmpty(buscar))
            {
                buscar = buscar.ToLower();
                query = query.Where(u =>
                    u.Username.ToLower().Contains(buscar) ||
                    u.Email.ToLower().Contains(buscar) ||
                    (u.Nombre != null && u.Nombre.ToLower().Contains(buscar)) ||
                    (u.Apellido != null && u.Apellido.ToLower().Contains(buscar)));
            }

            if (activo.HasValue)
            {
                query = query.Where(u => u.Activo == activo.Value);
            }

            // Ejecutar la consulta y mapear para no devolver las contraseñas
            var usuarios = await query
                .Select(u => new {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.Nombre,
                    u.Apellido,
                    u.FechaCreacion,
                    u.Activo
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            // Mapear para no devolver la contraseña
            var usuarioResponse = new
            {
                usuario.Id,
                usuario.Username,
                usuario.Email,
                usuario.Nombre,
                usuario.Apellido,
                usuario.FechaCreacion,
                usuario.Activo
            };

            return Ok(usuarioResponse);
        }

        // POST: api/Usuarios
        [HttpPost]
        [AllowAnonymous] // Permitir registro sin autenticación
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Verificar si ya existe un usuario con el mismo nombre de usuario o email
                if (await _context.Usuarios.AnyAsync(u => u.Username == usuario.Username))
                {
                    return Conflict(new { message = "El nombre de usuario ya está en uso" });
                }

                if (await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email))
                {
                    return Conflict(new { message = "El correo electrónico ya está registrado" });
                }

                // Encriptar la contraseña
                usuario.Password = BC.HashPassword(usuario.Password);

                // Establecer la fecha de creación
                usuario.FechaCreacion = DateTime.Now;

                // Agregar el usuario a la base de datos
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // Mapear para no devolver la contraseña
                var usuarioResponse = new
                {
                    usuario.Id,
                    usuario.Username,
                    usuario.Email,
                    usuario.Nombre,
                    usuario.Apellido,
                    usuario.FechaCreacion,
                    usuario.Activo
                };

                return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuarioResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el usuario: " + ex.Message });
            }
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest(new { message = "El ID no coincide con el usuario a actualizar" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Obtener el usuario existente
                var usuarioExistente = await _context.Usuarios.FindAsync(id);
                if (usuarioExistente == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                // Verificar si el username ya existe (para otro usuario)
                if (usuario.Username != usuarioExistente.Username &&
                    await _context.Usuarios.AnyAsync(u => u.Username == usuario.Username))
                {
                    return Conflict(new { message = "El nombre de usuario ya está en uso" });
                }

                // Verificar si el email ya existe (para otro usuario)
                if (usuario.Email != usuarioExistente.Email &&
                    await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email))
                {
                    return Conflict(new { message = "El correo electrónico ya está registrado" });
                }

                // Actualizar propiedades del usuario
                usuarioExistente.Username = usuario.Username;
                usuarioExistente.Email = usuario.Email;
                usuarioExistente.Nombre = usuario.Nombre;
                usuarioExistente.Apellido = usuario.Apellido;
                usuarioExistente.Activo = usuario.Activo;

                // Actualizar la contraseña solo si se proporciona una nueva
                if (!string.IsNullOrEmpty(usuario.Password))
                {
                    usuarioExistente.Password = BC.HashPassword(usuario.Password);
                }

                // Actualizar el usuario en la base de datos
                _context.Entry(usuarioExistente).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el usuario: " + ex.Message });
            }
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Usuario eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el usuario: " + ex.Message });
            }
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
