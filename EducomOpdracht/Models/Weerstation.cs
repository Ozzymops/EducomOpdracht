using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EducomOpdracht.Models
{
    public class Weerstation
    {
        public int Id { get; set; }
        public long StationId { get; set; }
        public DateTime Date { get; set; }
        public string Region { get; set; }
        public string Name { get; set; }
        public int Temperature { get; set; }
        public int Humidity { get; set; }
        public int AirPressure { get; set; }
    }
}
