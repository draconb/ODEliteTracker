using EliteJournalReader;
using EliteJournalReader.Events;
using ODEliteTracker.Database;
using ODEliteTracker.Models.Colonisation;
using ODEliteTracker.ViewModels.ModelViews.Colonisation;
using ODJournalDatabase.Database.Interfaces;
using ODJournalDatabase.JournalManagement;

namespace ODEliteTracker.Stores
{
    public sealed class ColonisationStore : LogProcessorBase
    {
        public ColonisationStore(IManageJournalEvents journalManager, IODDatabaseProvider databaseProvider)
        {
            this.journalManager = journalManager;
            this.databaseProvider = (ODEliteTrackerDatabaseProvider)databaseProvider;
            this.journalManager.RegisterLogProcessor(this);
        }

        private readonly IManageJournalEvents journalManager;
        private readonly ODEliteTrackerDatabaseProvider databaseProvider;
        private readonly Dictionary<Tuple<long, long, string>, ConstructionDepot> depots = [];
        private readonly Dictionary<long, CommanderSystem> commanderSystems = [];
        private HashSet<Tuple<long, long, string>> inactiveDepots = [];

        //When the colonisation events were added to the game
        private readonly DateTime colonisationEventUpdate = new(2025, 4, 7);

        private long CurrentSystemAddress;
        private string CurrentSystemName = "Unknown";
        private string CurrentStationName = "Unknown";

        public override string StoreName => "Colonisation";
        public IEnumerable<ConstructionDepot> Depots => depots.Values.OrderBy(x => x.SystemName);
        public IEnumerable<CommanderSystem> CommanderSystems => commanderSystems.Values.OrderBy(x => x.SystemName);

        public override Dictionary<JournalTypeEnum, bool> EventsToParse
        {
            get => new()
            {
                { JournalTypeEnum.Location,true},
                { JournalTypeEnum.FSDJump, true},
                { JournalTypeEnum.ColonisationBeaconDeployed, true},
                { JournalTypeEnum.ColonisationConstructionDepot, true},
                { JournalTypeEnum.ColonisationSystemClaim, true},
                { JournalTypeEnum.ColonisationSystemClaimRelease, true},
                { JournalTypeEnum.Docked, true},
                { JournalTypeEnum.Undocked, true},
                { JournalTypeEnum.Cargo, false},
            };
        }

        public EventHandler<ConstructionDepot>? OnNewDepot;
        public EventHandler<ConstructionDepot>? OnDepotUpdated;
        public EventHandler<CommanderSystem>? OnNewCommanderSystem;
        public EventHandler<CommanderSystem>? OnReleaseCommanderSystem;

        public override void ClearData()
        {
            depots.Clear();
            commanderSystems.Clear();
            inactiveDepots.Clear();
            CurrentSystemAddress = 0;
            CurrentSystemName = "Unknown";
            CurrentStationName = "Unknown";
            IsLive = false;
        }

        public override void Dispose()
        {
            this.journalManager.UnregisterLogProcessor(this);
        }

        public override void RunBeforeParsingHistory(int currentCmdrId)
        {
            inactiveDepots = databaseProvider.GetInactiveDepots();
        }
        public override DateTime GetJournalAge(DateTime defaultAge)
        {
            var ret = defaultAge > colonisationEventUpdate ? defaultAge : colonisationEventUpdate;

            return ret;
        }

        public override void ParseJournalEvent(JournalEntry evt)
        {
            if (EventsToParse.ContainsKey(evt.EventType) == false)
                return;

            switch (evt.EventData)
            {
                case LocationEvent.LocationEventArgs location:
                    CurrentSystemAddress = location.SystemAddress;
                    CurrentSystemName = location.StarSystem;
                    break;
                case FSDJumpEvent.FSDJumpEventArgs fsdJump:
                    CurrentSystemAddress = fsdJump.SystemAddress;
                    CurrentSystemName = fsdJump.StarSystem;
                    break;
                case ColonisationBeaconDeployedEvent.ColonisationBeaconDeployedEventArgs:
                    if (commanderSystems.ContainsKey(CurrentSystemAddress) == false)
                    {
                        var newClaim = new CommanderSystem(CurrentSystemAddress, CurrentSystemName);
                        commanderSystems.TryAdd(CurrentSystemAddress, newClaim);
                        TriggerNewCommanderSystemIfLive(newClaim);
                    }
                    break;
                case ColonisationSystemClaimEvent.ColonisationSystemClaimEventArgs colonisationSystemClaim:
                    if (commanderSystems.ContainsKey(colonisationSystemClaim.SystemAddress) == false)
                    {
                        var newClaim = new CommanderSystem(colonisationSystemClaim.SystemAddress, colonisationSystemClaim.StarSystem);
                        commanderSystems.TryAdd(colonisationSystemClaim.SystemAddress, newClaim);
                        TriggerNewCommanderSystemIfLive(newClaim);
                    }
                    break;
                case ColonisationSystemClaimReleaseEvent.ColonisationSystemClaimReleaseEventArgs colonisationSystemClaimRelease:
                    if (commanderSystems.TryGetValue(colonisationSystemClaimRelease.SystemAddress, out var system))
                    {
                        commanderSystems.Remove(colonisationSystemClaimRelease.SystemAddress);
                        if (IsLive)
                            OnReleaseCommanderSystem?.Invoke(this, system);
                    }
                    break;
                case ColonisationConstructionDepotEvent.ColonisationConstructionDepotEventArgs depot:
                    var key = Tuple.Create(depot.MarketID, CurrentSystemAddress, CurrentStationName);
                    if (depots.TryGetValue(key, out ConstructionDepot? value) && value.Update(depot, CurrentSystemAddress, CurrentSystemName, CurrentStationName))
                    {
                        TriggerDepotUpdateIfLive(value);
                        break;
                    }
                    var newDepot = new ConstructionDepot(depot, CurrentSystemAddress, CurrentSystemName, CurrentStationName, inactiveDepots.Contains(key));
                    if (depots.TryAdd(key, newDepot))
                    {
                        TriggerNewDepotIfLive(newDepot);
                    }
                    break;
                case DockedEvent.DockedEventArgs docked:
                    CurrentStationName = string.IsNullOrEmpty(docked.StationName_Localised) ? docked.StationName : docked.StationName_Localised;
                    break;
            }
        }

        private void TriggerNewCommanderSystemIfLive(CommanderSystem newClaim)
        {
            if (IsLive)
            {
                OnNewCommanderSystem?.Invoke(this, newClaim);
            }
        }

        private void TriggerDepotUpdateIfLive(ConstructionDepot value)
        {
            if (IsLive)
            {
                OnDepotUpdated?.Invoke(this, value);
            }
        }

        private void TriggerNewDepotIfLive(ConstructionDepot newDepot)
        {
            if (IsLive)
            {
                OnNewDepot?.Invoke(this, newDepot);
            }
        }

        internal void SetDepotActiveState(ConstructionDepotVM vM)
        {
            SetDepot(vM);
            inactiveDepots = databaseProvider.GetInactiveDepots();

            if (depots.TryGetValue(Tuple.Create(vM.MarketID, vM.SystemAddress, vM.StationName), out var depot))
            {
                depot.Inactive = vM.Inactive;
            }
        }

        private void SetDepot(ConstructionDepotVM vM)
        {
            if (vM.Inactive)
            {
                databaseProvider.AddInactiveDepot(vM.MarketID, vM.SystemAddress, vM.StationName);
                return;
            }

            databaseProvider.RemoveInactiveDepot(vM.MarketID, vM.SystemAddress, vM.StationName);
        }
    }
}
