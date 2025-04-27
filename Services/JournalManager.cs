using EliteJournalReader.Events;
using ODEliteTracker.Stores;
using ODJournalDatabase.Database.Interfaces;
using ODJournalDatabase.JournalManagement;

namespace ODEliteTracker.Services
{
    public sealed class JournalManager : IManageJournalEvents
    {
        #region ctor
        public JournalManager(JournalEventParser eventParser,
                              SettingsStore settingsStore,
                              IODDatabaseProvider oDDatabase)
        {
            //Event parser
            this.eventParser = eventParser;
            this.settingsStore = settingsStore;
            this.oDDatabase = oDDatabase;
            this.eventParser.OnJournalEventReceived += EventParser_OnJournalEventReceived;
            this.eventParser.LiveStatusChange += async (sender, e) => await EventParser_LiveStatusChange(sender, e);
        }
        #endregion

        private readonly JournalEventParser eventParser;
        private readonly SettingsStore settingsStore;
        private readonly IODDatabaseProvider oDDatabase;
        private readonly List<IProcessJournalLogs> journalLogParserList = [];

        private bool ManagerLive { get; set; }

        public List<JournalCommander> Commanders { get; private set; } = [];

        private JournalCommander? selectedCommander;
        public JournalCommander? SelectedCommander
        {
            get => selectedCommander;
            private set
            {
                selectedCommander = value;
                if (selectedCommander != null)
                    OnCommanderChanged?.Invoke(this, selectedCommander);
            }
        }

        public EventHandler<JournalCommander>? OnCommanderChanged;
        public event EventHandler? OnCommandersUpdated;

        #region Event Parser Methods
        private void EventParser_OnJournalEventReceived(object? sender, JournalEntry e)
        {
            if (e.CommanderID != settingsStore.SelectedCommanderID)
            {
                return;
            }

            foreach (var parser in journalLogParserList)
            {
                parser.ParseJournalEvent(e);
            }
        }

        private async Task EventParser_LiveStatusChange(object? sender, bool e)
        {
            if (e)
            {
                if (settingsStore.SelectedCommanderID <= 0)
                {
                    Commanders = await oDDatabase.GetAllJournalCommanders();
                    SelectedCommander = Commanders.FirstOrDefault();

                    if(SelectedCommander == null)
                    {
                        return;
                    }

                    settingsStore.SelectedCommanderID = SelectedCommander.Id;
                }

                ManagerLive = true;
                foreach (var parser in journalLogParserList)
                {
                    parser.ClearData();
                    parser.RunBeforeParsingHistory(settingsStore.SelectedCommanderID);
                }

                var history = journalLogParserList.Where(x => x.EventsToParse.Count != 0).ToList();

                await eventParser.StreamJournalHistoryOfTypeAsync(settingsStore.SelectedCommanderID, history, settingsStore.JournalAgeDateTime);                
            }
        }
        #endregion

        #region IManageJournalEvents Implementation
        public bool IsLive => ManagerLive;

        public async Task RegisterLogProcessor(IProcessJournalLogs logProcesser)
        {
            journalLogParserList.Add(logProcesser);

            if (IsLive)
            {
                logProcesser.ClearData();
                logProcesser.RunBeforeParsingHistory(settingsStore.SelectedCommanderID);

               await Task.Run(async () => await eventParser.StreamJournalHistoryOfTypeAsync(settingsStore.SelectedCommanderID, [logProcesser], settingsStore.JournalAgeDateTime));
            }
        }

        public void UnregisterLogProcessor(IProcessJournalLogs logProcesser)
        {
            journalLogParserList.Remove(logProcesser);
        }

        public async Task Initialise()
        {
            ManagerLive = false;
            Commanders = await oDDatabase.GetAllJournalCommanders();
            await ReadSelectedCommander();
        }

        public async Task ChangeCommander()
        {
            ManagerLive = false;
            await ReadSelectedCommander();
        }

        public async Task ReadNewDirectory(string path)
        {
            ManagerLive = false;
            var commander = new JournalCommander(-1, "Reading History", path, "", false);

            await eventParser.StartWatchingAsync(commander).ConfigureAwait(true);
            await UpdateCommanders();
        }

        public MarketInfo? GetMarketInfo()
        {
            if (ManagerLive == false)
                return null;

            return eventParser.GetMarketInfo(settingsStore.SelectedCommanderID);
        }

        public CargoEvent.CargoEventArgs? GetCargo()
        {
            if (ManagerLive == false)
                return null;

            return eventParser.GetCargo(settingsStore.SelectedCommanderID);
        }

        public async Task UpdateCommanders()
        {
            var commanders = await oDDatabase.GetAllJournalCommanders().ConfigureAwait(true);
            Commanders.Clear();
            Commanders.AddRange(commanders);

            //If we haven't found any commanders yet, set the first one
            if (settingsStore.SelectedCommanderID <= 0 && Commanders.Count != 0)
            {
                var ret = Commanders.FirstOrDefault();
                settingsStore.SelectedCommanderID = ret?.Id ?? 0;
            }
            OnCommandersUpdated?.Invoke(this, EventArgs.Empty);
        }
        public async Task ResetDatabase()
        {
            ManagerLive = false;
            eventParser.StopWatcher();
            Commanders.Clear();
            OnCommandersUpdated?.Invoke(this, EventArgs.Empty);
            await Task.Factory.StartNew(oDDatabase.ResetDatabaseAsync);
        }
        #endregion

        public async Task ReadSelectedCommander()
        {
            SelectedCommander = Commanders.FirstOrDefault(cmdr => cmdr.Id == settingsStore.SelectedCommanderID)
                ?? new(-1, "Reading History", "", "", false);

            await eventParser.StartWatchingAsync(SelectedCommander).ConfigureAwait(true);
        }

    }
}
