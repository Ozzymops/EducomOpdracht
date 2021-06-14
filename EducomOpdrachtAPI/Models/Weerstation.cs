using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EducomOpdrachtAPI.Models
{
    public class Weerstation
    {
        /// <summary>
        /// Een entry in de database.
        /// Weerstation vanuit https://data.buienradar.nl/1.1/feed/json opgehaald. Opgeslagen data bestaat uit station id, stationregio en stationnaam.
        /// Wordt gebruikt voor een dropdown menu in de andere applicatie.
        /// </summary>
        public long Id { get; set; }
        public string Region { get; set; }
        public string Name { get; set; }
    }
}
