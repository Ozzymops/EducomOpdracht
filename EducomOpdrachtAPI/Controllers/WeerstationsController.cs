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
    public class WeerstationsController : ControllerBase
    {
        private readonly WeerstationContext _context;

        public WeerstationsController(WeerstationContext context)
        {
            _context = context;
        }

        // GET: api/Weerstations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Weerstation>>> GetWeerstations()
        {
            return await _context.Weerstations.ToListAsync();
        }

        // GET: api/Weerstations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Weerstation>> GetWeerstation(long id)
        {
            var weerstation = await _context.Weerstations.FindAsync(id);

            if (weerstation == null)
            {
                return NotFound();
            }

            return weerstation;
        }

        // PUT: api/Weerstations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWeerstation(long id, Weerstation weerstation)
        {
            if (id != weerstation.Id)
            {
                return BadRequest();
            }

            _context.Entry(weerstation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WeerstationExists(id))
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

        // POST: api/Weerstations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Weerstation>> PostWeerstation(Weerstation weerstation)
        {
            _context.Weerstations.Add(weerstation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWeerstation", new { id = weerstation.Id }, weerstation);
        }

        // DELETE: api/Weerstations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeerstation(long id)
        {
            var weerstation = await _context.Weerstations.FindAsync(id);
            if (weerstation == null)
            {
                return NotFound();
            }

            _context.Weerstations.Remove(weerstation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WeerstationExists(long id)
        {
            return _context.Weerstations.Any(e => e.Id == id);
        }
    }
}
