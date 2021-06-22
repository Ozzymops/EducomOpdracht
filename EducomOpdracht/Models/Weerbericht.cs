using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EducomOpdracht.Models
{
    public class Weerbericht
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int MaxTemperature { get; set; }     // maximum temperatuur in graden Celsius
        public int MinTemperature { get; set; }     // minimum temperatuur in graden Celsius
        public int Windspeed { get; set; }          // windsnelheid volgens de schaal van Beaufort: https://www.knmi.nl/kennis-en-datacentrum/uitleg/windschaal-van-beaufort
        public int RainChance { get; set; }         // kans op regen in %
        public int SunChance { get; set; }          // kans op zon in %
    }
}
