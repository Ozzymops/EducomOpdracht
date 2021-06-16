using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EducomOpdrachtAPI.Models;

namespace EducomOpdrachtAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeerController : ControllerBase
    {
        private readonly WeerContext _context;

        public WeerController(WeerContext context)
        {
            _context = context;
        }

        // GET: api/Weer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Weer>>> GetWeer()
        {
            return await _context.Weerberichten.ToListAsync();
        }

        // GET: api/Weer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Weer>> GetWeer(long id)
        {
            var weerbericht = await _context.Weerberichten.FindAsync(id);

            if (weerbericht == null)
            {
                return NotFound();
            }

            return weerbericht;
        }

        // PUT: api/Weer/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWeer(long id, Weer weerbericht)
        {
            if (id != weerbericht.Id)
            {
                return BadRequest();
            }

            _context.Entry(weerbericht).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WeerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Weer
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Weer>> PostWeer(Weer weerbericht)
        {
            _context.Weerberichten.Add(weerbericht);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWeer), new { id = weerbericht.Id }, weerbericht);
        }

        // DELETE: api/Weer/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeer(long id)
        {
            var weerbericht = await _context.Weerberichten.FindAsync(id);
            if (weerbericht == null)
            {
                return NotFound();
            }

            _context.Weerberichten.Remove(weerbericht);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WeerExists(long id)
        {
            return _context.Weerberichten.Any(e => e.Id == id);
        }
    }
}
