using Microsoft.EntityFrameworkCore;
using EducomOpdrachtAPI.Models;

namespace EducomOpdrachtAPI.DAL
{
    /// <summary>
    /// Database context voor weerberichten. Dit is nodig om data op te kunnen slaan en op te kunnen halen vanuit een lokale database.
    /// </summary>
    public class WeerberichtContext : DbContext
    {
        public WeerberichtContext(DbContextOptions<WeerberichtContext> options) : base(options)
        {

        }

        public DbSet<Weerbericht> Weerberichten { get; set; }
    }
}
