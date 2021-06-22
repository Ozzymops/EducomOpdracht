using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EducomOpdracht.Models
{
    public class GraphModel
    {
        // Sent
        public long stationId;
        public DateTime startPeriod;
        public DateTime endPeriod;

        // General
        public int graphType = 0;

        // Weerstations
        public List<Weerstation> weerstations;
        public bool weerstationTemperature;
        public bool weerstationHumidity;
        public bool weerstationAirPressure;

        // Weerberichten
        public List<Weerbericht> weerberichten;
        public bool weerberichtMaxTemperature;
        public bool weerberichtMinTemperature;
        public bool weerberichtRainChance;
        public bool weerberichtSunChance;
    }
}
