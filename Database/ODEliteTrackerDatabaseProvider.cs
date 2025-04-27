using EFCore.BulkExtensions;
using EliteJournalReader;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ODJournalDatabase.Database.DTOs;
using ODJournalDatabase.Database.Interfaces;
using ODJournalDatabase.JournalManagement;

namespace ODEliteTracker.Database
{
    public sealed class ODEliteTrackerDatabaseProvider(ODEliteTrackerDbContextFactory contextFactory) : IODDatabaseProvider
    {
        private readonly ODEliteTrackerDbContextFactory _contextFactory = contextFactory;

        #region Commander Methods
        public async Task<List<JournalCommander>> GetAllJournalCommanders(bool includeHidden = false)
        {
            using var context = _contextFactory.CreateDbContext();

            if (!context.JournalCommanders.Any())
                return [];

            if (includeHidden)
            {
                var allCmdrs = await context.JournalCommanders
                    .Select(x => new JournalCommander(x.Id, x.Name, x.JournalDir, x.LastFile, x.IsHidden))
                    .ToListAsync();

                var reslt = allCmdrs.OrderBy(x => x.Name.Contains("(Legacy)"))
                                    .ThenBy(x => x.Name);
                return [.. reslt];
            }

            var cmdrs = await context.JournalCommanders
                .Where(x => x.IsHidden == false)
                .Select(x => new JournalCommander(x.Id, x.Name, x.JournalDir, x.LastFile, x.IsHidden))
                .ToListAsync();

            var ret = cmdrs.OrderBy(x => x.Name.Contains("(Legacy)"))
                            .ThenBy(x => x.Name);

            await context.Database.CloseConnectionAsync();
            return [.. ret];
        }

        public JournalCommander AddCommander(JournalCommander cmdr)
        {
            using var context = _contextFactory.CreateDbContext();

            var known = context.JournalCommanders.FirstOrDefault(x => x.Name == cmdr.Name);

            if (known == null)
            {
                known = new JournalCommanderDTO
                {
                    Name = cmdr.Name,
                    LastFile = cmdr.LastFile ?? string.Empty,
                    JournalDir = cmdr.JournalPath ?? string.Empty,
                    IsHidden = cmdr.IsHidden
                };
                context.JournalCommanders.Add(known);
                context.SaveChanges();
                return new(known.Id, known.Name, known.JournalDir, known.LastFile, known.IsHidden);
            }

            known.LastFile = cmdr.LastFile ?? string.Empty;
            known.JournalDir = cmdr.JournalPath ?? string.Empty;
            known.Name = cmdr.Name;
            known.IsHidden = cmdr.IsHidden;
            context.SaveChanges();
            context.Database.CloseConnection();
            return new(known.Id, known.Name, known.JournalDir, known.LastFile, known.IsHidden);
        }

        public JournalCommander? GetCommander(int cmdrId)
        {
            using var context = _contextFactory.CreateDbContext();

            var known = context.JournalCommanders.FirstOrDefault(x => x.Id == cmdrId);

            if (known == null)
            {
                return null;
            }

            return new(known.Id, known.Name, known.JournalDir, known.LastFile, known.IsHidden);
        }

        public async Task DeleteCommander(int commanderID)
        {
            using var context = _contextFactory.CreateDbContext();

            var cmdr = context.JournalCommanders.FirstOrDefault(x => x.Id == commanderID);

            if (cmdr == null)
            {
                return;
            }

            await context.JournalEntries.Where(x => x.CommanderID == commanderID).ExecuteDeleteAsync().ConfigureAwait(true);

            context.JournalCommanders.Remove(cmdr);

            await context.SaveChangesAsync(true);
        }
        #endregion

        #region Journal Methods
        public void AddJournalEntries(List<JournalEntry> journalEntries)
        {
            var entriesToAdd = journalEntries
                .OrderBy(x => x.Filename)
                .ThenBy(x => x.Offset)
                .Select(x => new JournalEntryDTO(x.Filename,
                                                 x.Offset,
                                                 x.TimeStamp,
                                                 x.CommanderID,
                                                 (int)x.EventType,
                                                 x.OriginalEvent?.ToString(Formatting.None) ?? string.Empty)
              
                ).ToArray();


            using var context = _contextFactory.CreateDbContext();

            context.BulkInsertOrUpdate(entriesToAdd, new BulkConfig() { PropertiesToIncludeOnCompare = ["TimeStamp", "Offset"] });
  
            var connection = context.Database.GetDbConnection();
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "pragma optimize;";
                command.ExecuteNonQuery();
            }
        }

        public async Task<List<JournalEntry>> GetAllJournalEntries(int cmdrId)
        {
            using var context = _contextFactory.CreateDbContext();

            var ret = await context.JournalEntries
                .Where(x => x.CommanderID == cmdrId)
                .OrderBy(x => x.TimeStamp)
                .ThenBy(x => x.Offset)
                .Select(x => new JournalEntry(
                    x.Filename,
                    x.Offset,
                    x.CommanderID,
                    (JournalTypeEnum)x.EventTypeId,
                    JournalWatcher.GetEventData(x.EventData),
                    null))
                .ToListAsync();

            return ret;
        }

        public async Task<List<JournalEntry>> GetJournalEntriesOfType(int cmdrId, List<JournalTypeEnum> types)
        {
            return await GetJournalEntriesOfType(cmdrId, types, DateTime.MinValue);
        }

        public async Task GetJournalsStream(int cmdrId, IEnumerable<JournalTypeEnum> types, DateTime age, Func<JournalEntry, Task> callBack)
        {
            using var context = _contextFactory.CreateDbContext();
            var journals = context.JournalEntries.Where(x => x.CommanderID == cmdrId
                            && x.TimeStamp.Date >= age.Date
                            && types.Contains((JournalTypeEnum)x.EventTypeId))
                .OrderBy(x => x.TimeStamp)
                .ThenBy(x => x.Offset)
                .AsNoTrackingWithIdentityResolution();
            await StreamJournals(journals, callBack);
        }

        private static async Task StreamJournals(IQueryable<JournalEntryDTO> journals, Func<JournalEntry, Task> callBack)
        {
            foreach (var x in journals)
            {
                var entry = new JournalEntry(
                    x.Filename,
                    x.Offset,
                    x.CommanderID,
                    (JournalTypeEnum)x.EventTypeId,
                    JournalWatcher.GetEventData(x.EventData),
                    null);

                await callBack(entry);
            }
        }

        public async Task<List<JournalEntry>> GetJournalEntriesOfType(int cmdrId, List<JournalTypeEnum> types, DateTime age)
        {
            if (types.Contains(JournalTypeEnum.Fileheader) == false)
                types.Add(JournalTypeEnum.Fileheader);

            using var context = _contextFactory.CreateDbContext();

            var ret = await context.JournalEntries
                .Where(x => x.CommanderID == cmdrId
                            && x.TimeStamp.Date >= age.Date
                            && types.Contains((JournalTypeEnum)x.EventTypeId))
                .OrderBy(x => x.TimeStamp)
                .ThenBy(x => x.Offset)
                .Select(x => new JournalEntry(
                    x.Filename,
                    x.Offset,
                    x.CommanderID,
                    (JournalTypeEnum)x.EventTypeId,
                    JournalWatcher.GetEventData(x.EventData),
                    null))
                .ToListAsync();

            var firstHeader = ret.FirstOrDefault(x => x.EventType == JournalTypeEnum.Fileheader);

            if (firstHeader != null)
            {
                var index = ret.IndexOf(firstHeader);

                if (index > 0)
                {
                    ret.RemoveRange(0, index);
                }
            }
            return ret;
        }

        public Task ParseJournalEventsOfType(int cmdrId, List<JournalTypeEnum> types, Action<JournalEntry> callback, DateTime age)
        {
            using var context = _contextFactory.CreateDbContext();

            var entries = context.JournalEntries
                .Where(x => x.TimeStamp >= age && cmdrId == x.CommanderID)
                .EventTypeCompare(types)
                .OrderBy(x => x.TimeStamp)
                .ThenBy(x => x.Offset);

            foreach (var e in entries)
            {
                callback.Invoke(new JournalEntry(
                    e.Filename,
                    e.Offset,
                    e.CommanderID,
                    (JournalTypeEnum)e.EventTypeId,
                    JournalWatcher.GetEventData(e.EventData),
                    null));
            }
            return Task.CompletedTask;
        }

        public HashSet<string> GetAllReadFilenames()
        {
            using var context = _contextFactory.CreateDbContext();

            var entries = context.JournalEntries
                .Select(x => x.Filename)
                .Distinct()
                .ToHashSet();

            return entries;
        }
        #endregion

        #region Settings
        public List<SettingsDTO> GetAllSettings()
        {
            using var context = _contextFactory.CreateDbContext();

            return [.. context.Settings];
        }

        public void AddSettings(List<SettingsDTO> settings)
        {
            using var context = _contextFactory.CreateDbContext();

            context.Settings.
                UpsertRange(settings)
                .On(x => x.Id)
                .Run();

            context.SaveChanges();

            context.Database.CloseConnection();
        }

        public void AddSetting(SettingsDTO settings)
        {
            using var context = _contextFactory.CreateDbContext();

            context.Settings.
                Upsert(settings)
                .On(x => x.Id)
                .Run();

            context.SaveChanges();
        }
        #endregion

        #region Database
        public async Task ResetDatabaseAsync()
        {
            using var context = _contextFactory.CreateDbContext();

            await context.Database.ExecuteSqlRawAsync(
                "DELETE FROM JournalCommanders;" +
                "DELETE FROM JournalEntries;" +
                "DELETE FROM InactiveDepots;" +
                "DELETE FROM SQLITE_SEQUENCE WHERE name='CommanderIgnoredSystems';" +
                "DELETE FROM SQLITE_SEQUENCE WHERE name='CartoIgnoredSystems';" +
                "DELETE FROM SQLITE_SEQUENCE WHERE name='InactiveDepots';");

            await context.SaveChangesAsync();
            context.Database.CloseConnection();
        }
        #endregion

        #region Colonisation
        public HashSet<Tuple<long, long, string>> GetInactiveDepots()
        {
            using var context = _contextFactory.CreateDbContext();

            return [.. context.InactiveDepots.Select(x => Tuple.Create(x.MarketID, x.SystemAddress, x.StationName))];
        }

        public void AddInactiveDepot(long marketID, long systemAddress, string stationName)
        {
            using var context = _contextFactory.CreateDbContext();
            context.InactiveDepots.
                Upsert(new DTOs.InactiveDepotsDTO(marketID, systemAddress, stationName))
                .Run();

            context.SaveChanges();
        }

        public void RemoveInactiveDepot(long marketID, long systemAddress, string stationName)
        {
            using var context = _contextFactory.CreateDbContext();

            var knownDepot = context.InactiveDepots.FirstOrDefault(x =>x.MarketID == marketID && x.SystemAddress == systemAddress && x.StationName == stationName);

            if (knownDepot != null)
            {
                context.InactiveDepots.Remove(knownDepot);
                context.SaveChanges();
            }
        }
        #endregion
    }
}
