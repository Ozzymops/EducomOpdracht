using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EducomOpdrachtAPI.DAL;
using EducomOpdrachtAPI.Models;
using System;
using Microsoft.AspNetCore.Routing;

namespace EducomOpdrachtAPI.Controllers
{
    [Route("api/[controller]")]
    public class WeerstationsController : ControllerBase
    {
        // Stelt de database context in (Entity Framework)
        private readonly WeerstationContext _context;

        public WeerstationsController(WeerstationContext context)
        {
            _context = context;
        }

        // GET: api/Weerstations
        /// <summary>
        /// Haalt alle weerstations op uit de database.
        /// </summary>
        /// <returns>Lijst met weerstations</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Weerstation>>> GetWeerstations()
        {
            return await _context.Weerstations.ToListAsync();
        }

        // GET: api/Weerstations/5
        /// <summary>
        /// Haalt een specifiek weerstation op.
        /// </summary>
        /// <param name="id">ID van weerstation</param>
        /// <returns>Weerstation</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Weerstation>> GetWeerstation(long id)
        {
            // Zoek weerstation
            var weerstation = await _context.Weerstations.FindAsync(id);

            // Als entry niet bestaat, doe niks
            if (weerstation == null)
            {
                return NotFound();
            }

            return weerstation;
        }

        // GET: api/Weerstations/bydate/startPeriod/endPeriod
        /// <summary>
        /// Haalt alle weerstation entries op die binnen de start- en einddatum vallen.
        /// </summary>
        /// <param name="startPeriod">Gegeven startdatum, alles voor deze datum wordt weggelaten</param>
        /// <param name="endPeriod">Gegeven einddatum, alles na deze datum wordt weggelaten</param>
        /// <returns>Lijst (IEnumerable) met weerstations</returns>
        [HttpGet("bydate/{stationId}/{startPeriod}/{endPeriod}")]
        public async Task<ActionResult<IEnumerable<Weerstation>>> GetWeerstationWithEnd(long stationId, string startPeriod, string endPeriod)
        {
            // Converteert datum strings naar daadwerkelijke DateTime
            DateTime startPeriodDate = DateTime.Parse(startPeriod);
            DateTime endPeriodDate = DateTime.Parse(endPeriod);

            List<Weerstation> weerstations = new List<Weerstation>();

            // Probeert alle weerstations op te halen binnen de datumrange
            try
            {
                weerstations = await _context.Weerstations.Where(o => o.StationId == stationId && o.Date >= startPeriodDate && o.Date <= endPeriodDate).ToListAsync();
            }
            catch
            {
                return NotFound();
            }

            // Als er geen zijn gevonden, return een NotFound
            if (weerstations.Count == 0)
            {
                return NotFound();
            }

            return weerstations;
        }

        // GET: api/Weerstations/bydate/startperiod
        /// <summary>
        /// Haalt alle weerstation entries op die binnen de start- en einddatum vallen, zonder gegeven einddatum.
        /// </summary>
        /// <param name="startPeriod">Gegeven startdatum, alles voor deze datum wordt weggelaten</param>
        /// <returns>Lijst (IEnumerable) met weerstations</returns> [HttpGet("bydate/{stationId}/{startPeriod}")]
        [HttpGet("bydate/{stationId}/{startPeriod}")]
        public async Task<ActionResult<IEnumerable<Weerstation>>> GetWeerstationWithoutEnd(long stationId, string startPeriod)
        {
            // Converteert datum strings naar daadwerkelijke DateTime
            DateTime startPeriodDate = DateTime.Parse(startPeriod);

            // Stel einddatum in als startdatum + 7 dagen
            DateTime endPeriodDate = startPeriodDate.AddDays(7);

            List<Weerstation> weerstations = new List<Weerstation>();

            // Probeert alle weerstations op te halen binnen de datumrange
            try
            {
                weerstations = await _context.Weerstations.Where(o => o.StationId == stationId && o.Date >= startPeriodDate && o.Date <= endPeriodDate).ToListAsync();
            }
            catch
            {
                return NotFound();
            }

            // Als er geen zijn gevonden, return een NotFound
            if (weerstations.Count == 0)
            {
                return NotFound();
            }

            return weerstations;
        }

        // PUT: api/Weerstations/5
        /// <summary>
        /// Update een weerstation entry met de gegeven waarden.
        /// </summary>
        /// <param name="id">ID van weerstation</param>
        /// <param name="weerstation">Nieuwe waarden voor weerstation</param>
        /// <returns>Niks</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWeerstation(long id, Weerstation weerstation)
        {
            // Authenticeer: alleen de console app en website mag toegang hebben tot deze functie
            Authenticator auth = new Authenticator();
            bool authenticated = auth.Authenticate(Request.Headers["Authorization"].ToString());

            // Als authenticatie succesvol is
            if (authenticated)
            {
                // Check of gegeven ID overeen komt met de ID van het weerstation
                if (id != weerstation.Id)
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
                _context.Entry(weerstation).State = EntityState.Modified;

                try
                {
                    // Sla de entry op
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
            }

            return NoContent();
        }

        // POST: api/Weerstations
        /// <summary>
        /// Maak een nieuw weerstation entry met de gegeven waarden
        /// </summary>
        /// <param name="weerstation">Waarden voor nieuw weerstation</param>
        /// <returns>GetWeerstation van nieuw aangemaakt weerstation</returns>
        [HttpPost]
        public async Task<ActionResult<Weerstation>> PostWeerstation(Weerstation weerstation)
        {
            // Authenticeer: alleen de console app en website mag toegang hebben tot deze functie
            Authenticator auth = new Authenticator();
            bool authenticated = auth.Authenticate(Request.Headers["Authorization"].ToString());

            // Als authenticatie succesvol is
            if (authenticated)
            {
                // Check of entry al bestaat
                if (!_context.Weerstations.Any(o => o.StationId == weerstation.StationId && o.Date == weerstation.Date))
                {
                    // Nee -> voeg toe (POST)
                    _context.Weerstations.Add(weerstation);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Ja -> update (PUT)
                    int id = _context.Weerstations.First(o => o.StationId == weerstation.StationId && o.Date == weerstation.Date).Id;
                    weerstation.Id = id;
                    await PutWeerstation((long)id, weerstation);
                }

                return CreatedAtAction(nameof(GetWeerstation), new { id = weerstation.Id }, weerstation);
            }

            return NoContent();
        }

        // POST: api/Weerstations/list
        /// <summary>
        /// Haalt data op uit de Buienradar feed en verwerkt maakt aan (POST) of update (PUT) deze in de database.
        /// </summary>
        /// <returns>Niks</returns>
        [HttpGet("list")]
        public async Task<ActionResult<List<Weerstation>>> GetAndPostWeerstationList()
        {
            // Authenticeer: alleen de console app en website mag toegang hebben tot deze functie
            Authenticator auth = new Authenticator();
            bool authenticated = auth.Authenticate(Request.Headers["Authorization"].ToString());

            // Als authenticatie succesvol is
            if (authenticated)
            {
                Models.Database database = new Models.Database();
                database.GetWeerstationsFromFeed();

                // Loop door lijst verkregen van Buienradar
                foreach (Weerstation weerstation in database.weerstations)
                {
                    // Check of entry al bestaat
                    if (!_context.Weerstations.Any(o => o.StationId == weerstation.StationId && o.Date == weerstation.Date))
                    {
                        // Nee: voeg toe (POST)
                        _context.Weerstations.Add(weerstation);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // Ja -> update (PUT)
                        int id = _context.Weerstations.First(o => o.StationId == weerstation.StationId && o.Date == weerstation.Date).Id;
                        weerstation.Id = id;
                        await PutWeerstation((long)id, weerstation);
                    }
                }
            }
      
            return NoContent();
        }

        // DELETE: api/Weerstations/5
        /// <summary>
        /// Delete een specifiek weerstation
        /// </summary>
        /// <param name="id">ID van weerstation</param>
        /// <returns>Niks</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeerstation(long id)
        {
            // Authenticeer: alleen de console app en website mag toegang hebben tot deze functie
            Authenticator auth = new Authenticator();
            bool authenticated = auth.Authenticate(Request.Headers["Authorization"].ToString());

            // Als authenticatie succesvol is
            if (authenticated)
            {
                // Zoek weerstation entry
                var weerstation = await _context.Weerstations.FindAsync(id);

                // Als entry niet bestaat, doe niks
                if (weerstation == null)
                {
                    return NotFound();
                }

                // Als entry bestaat, verwijder en sla op
                _context.Weerstations.Remove(weerstation);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

        // DELETE: api/Weerstations
        /// <summary>
        /// Delete alle weerstations
        /// </summary>
        /// <returns>Niks</returns>
        [HttpDelete]
        public async Task<ActionResult<Weerstation>> DeleteAllWeerstation()
        {
            // Authenticeer: alleen de console app en website mag toegang hebben tot deze functie
            Authenticator auth = new Authenticator();
            bool authenticated = auth.Authenticate(Request.Headers["Authorization"].ToString());

            // Als authenticatie succesvol is
            if (authenticated)
            {
                // Zoek weerstation entries
                var weerstationList = await _context.Weerstations.ToListAsync();

                // Als entries niet bestaan, doe niks
                if (weerstationList.Count == 0)
                {
                    return NotFound();
                }

                // Als entries bestaan, verwijder
                foreach (Weerstation weerstation in weerstationList)
                {
                    _context.Weerstations.Remove(weerstation);
                }

                // Sla wijzigingen op
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

        private bool WeerstationExists(long id)
        {
            return _context.Weerstations.Any(e => e.Id == id);
        }
    }
}
