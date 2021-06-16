using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducomOpdrachtTaskScheduler.Models
{
    class Weerbericht
    {
        public long Id { get; set; }
        public long StationId { get; set; }
        public DateTime Date { get; set; }
        public int Temperature { get; set; }
        public int Humidity { get; set; }
        public int AirPressure { get; set; }
    }
}
