using Newtonsoft.Json;
using ODEliteTracker.Database;
using ODEliteTracker.Database.DTOs;
using ODJournalDatabase.Database.Interfaces;
using ODMVVM.Helpers.IO;

namespace ODEliteTracker.Stores
{
    public sealed class TickDataStore
    {
        public TickDataStore(IODDatabaseProvider databaseProvider, SettingsStore settings) 
        {
            this.databaseProvider = (ODEliteTrackerDatabaseProvider)databaseProvider;
            this.settings = settings;
        }

        private List<BGSTickData> tickData = [];
        private readonly ODEliteTrackerDatabaseProvider databaseProvider;
        private readonly SettingsStore settings;

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

        private async Task<bool> CheckForNewTicks()
        {
            var min = 0L;

            tickData = await databaseProvider.GetTickData(settings.JournalAgeDateTime.AddDays(-7));

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

            string baseurl = $"https://elitebgs.app/api/ebgs/v5/";
            string url = $"ticks?timeMin={min}&timeMax={max}";

            var json = await Internet.GetJsonFromUrl(baseurl, url);

            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            List<BGSTickData>? data = JsonConvert.DeserializeObject<List<BGSTickData>>(json);

            return data;
        }
    }
}
