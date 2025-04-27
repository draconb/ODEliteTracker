using Microsoft.EntityFrameworkCore;
using ODEliteTracker.Database.DTOs;
using ODJournalDatabase.Database;
namespace ODEliteTracker.Database
{
    public sealed class ODEliteTrackerDbContext(DbContextOptions options) : JournalContextBase(options)
    {
        public DbSet<InactiveDepotsDTO> InactiveDepots { get; set; }
    }
}
