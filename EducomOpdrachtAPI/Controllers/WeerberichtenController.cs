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
    public class WeerberichtenController : ControllerBase
    {
        private readonly WeerberichtContext _context;

        public WeerberichtenController(WeerberichtContext context)
        {
            _context = context;
        }

        // GET: api/Weerberichten
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Weerbericht>>> GetWeerberichten()
        {
            return await _context.Weerberichten.ToListAsync();
        }

        // GET: api/Weerberichten/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Weerbericht>> GetWeerbericht(long id)
        {
            var weerbericht = await _context.Weerberichten.FindAsync(id);

            if (weerbericht == null)
            {
                return NotFound();
            }

            return weerbericht;
        }

        // PUT: api/Weerberichten/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWeerbericht(long id, Weerbericht weerbericht)
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
                if (!WeerberichtExists(id))
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

        // POST: api/Weerberichten
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Weerbericht>> PostWeerbericht(Weerbericht weerbericht)
        {
            if (!_context.Weerberichten.Any(o => o.Date == weerbericht.Date && o.StationId == weerbericht.StationId))
            {
                _context.Weerberichten.Add(weerbericht);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Redirect to PUT
            }

            // return CreatedAtAction("GetWeerbericht", new { id = weerbericht.Id }, weerbericht);
            return CreatedAtAction(nameof(GetWeerbericht), new { id = weerbericht.Id }, weerbericht);
        }

        // DELETE: api/Weerberichten/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeerbericht(long id)
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

        private bool WeerberichtExists(long id)
        {
            return _context.Weerberichten.Any(e => e.Id == id);
        }
    }
}
