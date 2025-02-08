using Microsoft.EntityFrameworkCore;
using SCADAFrontend.Models;

namespace SCADAFrontend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<RtuData> RtuReadings { get; set; }
        public DbSet<HistoricalData> HistoricalReadings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=YOUR_SERVER_NAME;Database=SCADA_DB;Trusted_Connection=True;");
            }
        }
    }
}

