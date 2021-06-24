using System;
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
        // Stelt de database context in (Entity Framework)
        private readonly WeerberichtContext _context;

        public WeerberichtenController(WeerberichtContext context)
        {
            _context = context;
        }

        // GET: api/Weerberichten
        /// <summary>
        /// Haalt alle weerberichten op uit de database.
        /// </summary>
        /// <returns>Lijst met weerberichten</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Weerbericht>>> GetWeerberichten()
        {
            return await _context.Weerberichten.ToListAsync();
        }

        // GET: api/Weerberichten/5
        /// <summary>
        /// Haalt een specifiek weerbericht op.
        /// </summary>
        /// <param name="id">ID van weerbericht</param>
        /// <returns>Weerbericht</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Weerbericht>> GetWeerbericht(long id)
        {
            // Zoek weerbericht
            var weerbericht = await _context.Weerberichten.FindAsync(id);

            // Als entry niet bestaat, doe niks
            if (weerbericht == null)
            {
                return NotFound();
            }

            return weerbericht;
        }

        // GET: api/Weerberichten/bydate/startPeriod/endPeriod
        /// <summary>
        /// Haalt alle weerberichten op die binnen de start- en einddatum vallen.
        /// </summary>
        /// <param name="startPeriod">Gegeven startdatum, alles voor deze datum wordt weggelaten</param>
        /// <param name="endPeriod">Gegeven einddatum, alles na deze datum wordt weggelaten</param>
        /// <returns>Lijst (IEnumerable) met weerberichten</returns>
        [HttpGet("bydate/{startPeriod}/{endPeriod}")]
        public async Task<ActionResult<IEnumerable<Weerbericht>>> GetWeerberichtWithEnd(string startPeriod, string endPeriod)
        {
            // Converteert datum strings naar daadwerkelijke DateTime
            DateTime startPeriodDate = DateTime.Parse(startPeriod);
            DateTime endPeriodDate = DateTime.Parse(endPeriod);

            List<Weerbericht> weerberichten = new List<Weerbericht>();

            // Probeert alle weerberichten op te halen binnen de datumrange
            try
            {
                weerberichten = await _context.Weerberichten.Where(o => o.Date >= startPeriodDate && o.Date <= endPeriodDate).ToListAsync();
            }
            catch
            {
                return NotFound();
            }

            // Als er geen zijn gevonden, return een NotFound
            if (weerberichten.Count == 0)
            {
                return NotFound();
            }

            return weerberichten;
        }

        // GET: api/Weerberichten/bydate/startperiod
        /// <summary>
        /// Haalt alle weerberichten op die binnen de start- en einddatum vallen, zonder gegeven einddatum.
        /// </summary>
        /// <param name="startPeriod">Gegeven startdatum, alles voor deze datum wordt weggelaten</param>
        /// <returns>Lijst (IEnumerable) met weerberichten</returns>
        [HttpGet("bydate/{startPeriod}")]
        public async Task<ActionResult<IEnumerable<Weerbericht>>> GetWeerberichtWithoutEnd(string startPeriod)
        {
            // Converteert datum strings naar daadwerkelijke DateTime
            DateTime startPeriodDate = DateTime.Parse(startPeriod);

            // Stel einddatum in als startdatum + 7 dagen
            DateTime endPeriodDate = startPeriodDate.AddDays(7);

            List<Weerbericht> weerberichten = new List<Weerbericht>();

            // Probeert alle weerberichten op te halen binnen de datumrange
            try
            {
                weerberichten = await _context.Weerberichten.Where(o => o.Date >= startPeriodDate && o.Date <= endPeriodDate).ToListAsync();
            }
            catch
            {
                return NotFound();
            }

            // Als er geen zijn gevonden, return een NotFound
            if (weerberichten.Count == 0)
            {
                return NotFound();
            }

            return weerberichten;
        }

        // PUT: api/Weerberichten/5
        /// <summary>
        /// Update een weerbericht entry met de gegeven waarden.
        /// </summary>
        /// <param name="id">ID van weerbericht</param>
        /// <param name="weerbericht">Nieuwe waarden voor weerbericht</param>
        /// <returns>Niks</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWeerbericht(long id, Weerbericht weerbericht, string requestHeader)
        {
            // Authenticeer: alleen de console app en website mag toegang hebben tot deze functie
            Authenticator auth = new Authenticator();
            bool authenticated = auth.Authenticate(Request.Headers["Authorization"].ToString());

            if (authenticated)
            {
                // Check of gegeven ID overeen komt met de ID van het weerbericht
                if (id != weerbericht.Id)
                {
                    return BadRequest();
                }

                // Zoekt in de database of entry bestaat
                var local = _context.Set<Weerbericht>().Local.FirstOrDefault(o => o.Id.Equals((int)id));

                // Als entry bestaat, Detach de context zodat dit aangepast kan worden
                if (local != null)
                {
                    _context.Entry(local).State = EntityState.Detached;
                }

                // Pas de entry aan
                _context.Entry(weerbericht).State = EntityState.Modified;

                try
                {
                    // Sla de entry op
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
            }
            
            return NoContent();
        }

        // POST: api/Weerberichten
        /// <summary>
        /// Maak een nieuw weerbericht entry met de gegeven waarden
        /// </summary>
        /// <param name="weerbericht">Waarden voor nieuw weerbericht</param>
        /// <returns>GetWeerbericht van nieuw aangemaakt weerbericht</returns>
        [HttpPost]
        public async Task<ActionResult<Weerbericht>> PostWeerbericht(Weerbericht weerbericht)
        {
            // Authenticeer: alleen de console app en website mag toegang hebben tot deze functie
            Authenticator auth = new Authenticator();
            bool authenticated = auth.Authenticate(Request.Headers["Authorization"].ToString());

            // Als authenticatie succesvol is
            if (authenticated)
            {
                // Check of entry al bestaat
                if (!_context.Weerberichten.Any(o => o.Date == weerbericht.Date))
                {
                    // Nee -> voeg toe (POST)
                    _context.Weerberichten.Add(weerbericht);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Ja -> update (PUT)
                    int id = _context.Weerberichten.First(o => o.Date == weerbericht.Date).Id;
                    weerbericht.Id = id;
                    await PutWeerbericht((long)id, weerbericht, Request.Headers["Authorization"].ToString());
                }

                return CreatedAtAction(nameof(GetWeerbericht), new { id = weerbericht.Id }, weerbericht);
            }

            return NoContent();
        }

        // POST: api/Weerberichten/list
        /// <summary>
        /// Haalt data op uit de Buienradar feed en verwerkt maakt aan (POST) of update (PUT) deze in de database.
        /// </summary>
        /// <returns>Niks</returns>
        [HttpGet("list")]
        public async Task<ActionResult<List<Weerbericht>>> GetAndPostWeerberichtList()
        {
            // Authenticeer: alleen de console app en website mag toegang hebben tot deze functie
            Authenticator auth = new Authenticator();
            bool authenticated = auth.Authenticate(Request.Headers["Authorization"].ToString());

            // Als authenticatie succesvol is
            if (authenticated)
            {
                Models.Database database = new Models.Database();
                database.GetWeerberichtenFromFeed();

                // Loop door lijst verkregen van Buienradar
                foreach (Weerbericht weerbericht in database.weerberichten)
                {
                    // Check of entry al bestaat
                    if (!_context.Weerberichten.Any(o => o.Date == weerbericht.Date))
                    {
                        // Nee: voeg toe (POST)
                        _context.Weerberichten.Add(weerbericht);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // Ja -> update (PUT)
                        int id = _context.Weerberichten.First(o => o.Date == weerbericht.Date).Id;
                        weerbericht.Id = id;
                        await PutWeerbericht((long)id, weerbericht, Request.Headers["Authorization"].ToString());
                    }
                }
            }

            return NoContent();
        }

        // DELETE: api/Weerberichten/5
        /// <summary>
        /// Delete een specifiek weerbericht
        /// </summary>
        /// <param name="id">ID van weerbericht</param>
        /// <returns>Niks</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeerbericht(long id)
        {
            // Authenticeer: alleen de console app en website mag toegang hebben tot deze functie
            Authenticator auth = new Authenticator();
            bool authenticated = auth.Authenticate(Request.Headers["Authorization"].ToString());

            // Als authenticatie succesvol is
            if (authenticated)
            {
                // Zoek weerbericht entry
                var weerbericht = await _context.Weerberichten.FindAsync(id);

                // Als entry niet bestaat, doe niks
                if (weerbericht == null)
                {
                    return NotFound();
                }

                // Als entry bestaat, verwijder en sla op
                _context.Weerberichten.Remove(weerbericht);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

        // DELETE: api/Weerstations
        /// <summary>
        /// Delete alle weerberichten
        /// </summary>
        /// <returns>Niks</returns>
        [HttpDelete]
        public async Task<ActionResult<Weerbericht>> DeleteAllWeerbericht()
        {
            // Authenticeer: alleen de console app en website mag toegang hebben tot deze functie
            Authenticator auth = new Authenticator();
            bool authenticated = auth.Authenticate(Request.Headers["Authorization"].ToString());

            // Als authenticatie succesvol is
            if (authenticated)
            {
                // Zoek weerbericht entries
                var weerberichtList = await _context.Weerberichten.ToListAsync();

                // Als entries niet bestaan, doe niks
                if (weerberichtList.Count == 0)
                {
                    return NotFound();
                }

                // Als entries bestaan, verwijder
                foreach (Weerbericht weerbericht in weerberichtList)
                {
                    _context.Weerberichten.Remove(weerbericht);
                }

                // Sla wijzigingen op
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
