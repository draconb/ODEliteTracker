using ODEliteTracker.Database;
using ODEliteTracker.Database.DTOs;
using ODEliteTracker.Services;
using ODJournalDatabase.Database.Interfaces;

namespace ODEliteTracker.Stores
{
    public sealed class TickDataStore
    {
        public TickDataStore(IODDatabaseProvider databaseProvider, SettingsStore settings, EliteBGSApiService eliteBGS) 
        {
            this.databaseProvider = (ODEliteTrackerDatabaseProvider)databaseProvider;
            this.settings = settings;
            this.eliteBGS = eliteBGS;
        }

        private List<BGSTickData> tickData = [];
        private readonly ODEliteTrackerDatabaseProvider databaseProvider;
        private readonly SettingsStore settings;
        private readonly EliteBGSApiService eliteBGS;

        public List<BGSTickData> BGSTickData => tickData;

        public EventHandler? NewTick;
        public async Task Initialise()
        {
            await CheckForNewTicks();
        }

        public void CheckForNewTick()
        {
            _= Task.Factory.StartNew(async () => 
            {
                var newTick = await CheckForNewTicks();
                if(newTick)
                    NewTick?.Invoke(this, EventArgs.Empty);
            }).ConfigureAwait(false);
        }

        public async Task UpdateTickFromDatabase()
        {
            tickData = await databaseProvider.GetTickData(settings.JournalAgeDateTime.AddDays(-7));
        }

        private async Task<bool> CheckForNewTicks()
        {
            var min = 0L;

            await UpdateTickFromDatabase();

            if (tickData.Count != 0)
            {
                //Get Unix Milliseconds and add a minute so we don't get the same tick again
                min = ((DateTimeOffset)tickData.First().Updated_At.AddMinutes(1)).ToUnixTimeMilliseconds();
            }

            var newTicks = await GetLatestTickHistory(min).ConfigureAwait(true);

            if (newTicks is null || newTicks.Count == 0)
                return false;

            var newTickData = newTicks.Except(tickData);

            if (newTickData.Any())
            {
                await databaseProvider.AddTickData(newTickData);
                tickData = await databaseProvider.GetTickData(settings.JournalAgeDateTime.AddDays(-7));
                return true;
            }

            return false;
        }

        private async Task<List<BGSTickData>?> GetLatestTickHistory(long min)
        {
            if(min == 0)
            {
                min = DateTimeOffset.Parse("2018-01-01T00:00:00.000Z").ToUnixTimeMilliseconds();
            }

            long max = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var data = await eliteBGS.GetTicks(min, max);

            return data;
        }

        internal async Task<BGSTickData> AddTick(DateTime dateTime)
        {
            var newTick = new BGSTickData(DateTime.UtcNow.Ticks.ToString(), dateTime, dateTime, true);

            await databaseProvider.AddTickData([newTick]).ConfigureAwait(true);
            tickData = await databaseProvider.GetTickData(settings.JournalAgeDateTime.AddDays(-7)).ConfigureAwait(true);

            return newTick;
        }

        internal async Task DeleteTick(string iD)
        {
            await databaseProvider.DeleteTickData(iD).ConfigureAwait(true);
            await UpdateTickFromDatabase();
        }
    }
}
