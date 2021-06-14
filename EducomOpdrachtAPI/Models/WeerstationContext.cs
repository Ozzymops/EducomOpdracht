using Microsoft.EntityFrameworkCore;

namespace EducomOpdrachtAPI.Models
{
    /// <summary>
    /// Database context voor weerberichten. Dit is nodig om data op te kunnen slaan en op te kunnen halen vanuit een lokale database.
    /// </summary>
    public class WeerstationContext : DbContext
    {
        public WeerstationContext(DbContextOptions<WeerstationContext> options) : base(options)
        {

        }

        public DbSet<Weerstation> Weerstations { get; set; }
    }
}
