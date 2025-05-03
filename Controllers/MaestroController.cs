using API_Usuarios.Data;
using API_Usuarios.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Usuarios.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaestroController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MaestroController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/maestros
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Maestros>>> GetMaestros([FromQuery] string nombre)
        {
            var query = _context.Maestros.AsQueryable();

            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(m => m.Nombre.Contains(nombre));
            }   

            var resultados = await query.ToListAsync();
            return Ok(resultados);
        }

        // GET: api/maestros/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Maestros>> GetMaestro(int id)
        {
            var maestro = await _context.Maestros.FindAsync(id);

            if (maestro == null)
                return NotFound();

            return maestro;
        }

        // POST: api/maestros
        [HttpPost]
        public async Task<ActionResult<Maestros>> CreateMaestro(Maestros maestro)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Maestros.Add(maestro);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMaestro), new { id = maestro.Id }, maestro);
        }

        // PUT: api/maestros/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaestro(int id, Maestros maestro)
        {
            if (id != maestro.Id)
                return BadRequest("El ID de la URL no coincide con el del cuerpo");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Entry(maestro).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Maestros.Any(e => e.Id == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // DELETE: api/maestros/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaestro(int id)
        {
            var maestro = await _context.Maestros.FindAsync(id);
            if (maestro == null)
                return NotFound($"No se encontró un maestro con ID {id}");

            _context.Maestros.Remove(maestro);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
