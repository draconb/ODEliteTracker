using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace ODEliteTracker.Database
{
    public sealed class ODDesignTimeDbContextFactory : IDesignTimeDbContextFactory<ODEliteTrackerDbContext>
    {
        public ODEliteTrackerDbContext CreateDbContext(string[] args)
        {
            var dbOptions = new DbContextOptionsBuilder().UseSqlite("DataSource=DebugDatabase.db;").Options;
            return new ODEliteTrackerDbContext(dbOptions);
        }
    }
}
