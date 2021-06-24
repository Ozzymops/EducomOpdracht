﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EducomOpdrachtAPI.DAL;
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

        // GET: api/Weerberichten/bydate/startPeriod/endPeriod
        // Voor bijgegeven eindperiode
        [HttpGet("bydate/{startPeriod}/{endPeriod}")]
        public async Task<ActionResult<IEnumerable<Weerbericht>>> GetWeerberichtWithEnd(string startPeriod, string endPeriod)
        {
            DateTime startPeriodDate = DateTime.Parse(startPeriod);
            DateTime endPeriodDate = DateTime.Parse(endPeriod);

            List<Weerbericht> weerberichten = new List<Weerbericht>();

            try
            {
                weerberichten = await _context.Weerberichten.Where(o => o.Date >= startPeriodDate && o.Date <= endPeriodDate).ToListAsync();
            }
            catch
            {
                return NotFound();
            }

            if (weerberichten.Count == 0)
            {
                return NotFound();
            }

            return weerberichten;
        }

        // GET: api/Weerberichten/bydate/startperiod
        // Voor standaard +5 dagen eindperiode (zeven dagen is onmogelijk, buienradar heeft maar informatie tot op vijf dagen vooruit)
        [HttpGet("bydate/{startPeriod}")]
        public async Task<ActionResult<IEnumerable<Weerbericht>>> GetWeerberichtWithoutEnd(string startPeriod)
        {
            DateTime startPeriodDate = DateTime.Parse(startPeriod);
            DateTime endPeriodDate = startPeriodDate.AddDays(7);

            List<Weerbericht> weerberichten = new List<Weerbericht>();

            try
            {
                weerberichten = await _context.Weerberichten.Where(o => o.Date >= startPeriodDate && o.Date <= endPeriodDate).ToListAsync();
            }
            catch
            {
                return NotFound();
            }

            if (weerberichten.Count == 0)
            {
                return NotFound();
            }

            return weerberichten;
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

            var local = _context.Set<Weerbericht>().Local.FirstOrDefault(o => o.Id.Equals((int)id));

            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
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
            // Authenticeer: alleen de console app mag toegang hebben tot POST en PUT
            Authenticator auth = new Authenticator();
            bool authenticated = auth.Authenticate(Request.Headers["Authorization"].ToString());

            if (authenticated)
            {
                if (!_context.Weerberichten.Any(o => o.Date == weerbericht.Date))
                {
                    _context.Weerberichten.Add(weerbericht);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    int id = _context.Weerberichten.First(o => o.Date == weerbericht.Date).Id;
                    weerbericht.Id = id;
                    await PutWeerbericht((long)id, weerbericht);
                }

                return CreatedAtAction(nameof(GetWeerbericht), new { id = weerbericht.Id }, weerbericht);
            }

            return NoContent();
        }

        // POST: api/Weerberichten/list
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpGet("list")]
        public async Task<ActionResult<List<Weerbericht>>> GetAndPostWeerberichtList()
        {
            // Authenticeer: alleen de console app mag toegang hebben tot POST en PUT
            Authenticator auth = new Authenticator();
            bool authenticated = auth.Authenticate(Request.Headers["Authorization"].ToString());

            if (authenticated)
            {
                Models.Database database = new Models.Database();
                database.GetWeerberichtenFromFeed();

                foreach (Weerbericht weerbericht in database.weerberichten)
                {
                    if (!_context.Weerberichten.Any(o => o.Date == weerbericht.Date))
                    {
                        _context.Weerberichten.Add(weerbericht);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        int id = _context.Weerberichten.First(o => o.Date == weerbericht.Date).Id;
                        weerbericht.Id = id;
                        await PutWeerbericht((long)id, weerbericht);
                    }
                }
            }

            return NoContent();
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

        // DELETE: api/Weerstations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpDelete]
        public async Task<ActionResult<Weerbericht>> DeleteAllWeerbericht()
        {
            // Authenticeer: alleen de console app mag toegang hebben tot POST en PUT
            Authenticator auth = new Authenticator();
            bool authenticated = auth.Authenticate(Request.Headers["Authorization"].ToString());

            if (authenticated)
            {
                var weerberichtList = await _context.Weerberichten.ToListAsync();

                foreach (Weerbericht weerbericht in weerberichtList)
                {
                    _context.Weerberichten.Remove(weerbericht);
                }

                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

        private bool WeerberichtExists(long id)
        {
            return _context.Weerberichten.Any(e => e.Id == id);
        }
    }
}
