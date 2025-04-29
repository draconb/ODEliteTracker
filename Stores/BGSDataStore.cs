using EliteJournalReader.Events;
using EliteJournalReader;
using ODEliteTracker.Models.Missions;
using ODJournalDatabase.JournalManagement;
using ODEliteTracker.Models.Galaxy;
using ODEliteTracker.Database.DTOs;
using ODEliteTracker.Models.BGS;

namespace ODEliteTracker.Stores
{
    public sealed class BGSDataStore : LogProcessorBase
    {
        public BGSDataStore(IManageJournalEvents journalManager,
            TickDataStore tickData)
        {
            this.journalManager = journalManager;
            this.tickDataStore = tickData;

            this.journalManager.RegisterLogProcessor(this);

            this.tickDataStore.NewTick += OnNewTick;

            this.tickContainer = new(tickDataStore.BGSTickData);
        }

        private IManageJournalEvents journalManager;
        private readonly TickDataStore tickDataStore;
        private long CurrentSystemAddress;
        private string CurrentSystemName = "Unknown";
        private long CurrentMarketID;
        private string CurrentStationName = "Unknown";
        private bool odyssey;

        private Dictionary<string, FactionData> factions = [];
        private TickContainer tickContainer;
        private readonly List<BGSMission> missions = [];
        private readonly Dictionary<long, BGSStarSystem> systems = [];
        private TickData? selectedTick;
        private BGSStarSystem? currentSystem;

        public TickData? SelectedTick => selectedTick;
        public List<BGSTickData> TickData => tickContainer.TickData;
        public IEnumerable<BGSStarSystem> Systems => systems.Values;

        #region Events
        public EventHandler<BGSMission>? MissionAddedEvent;
        public EventHandler<BGSMission>? MissionUpdatedEvent;
        public EventHandler<BGSMission>? CargoDepotEvent;
        public EventHandler? MissionsUpdatedEvent;
        public EventHandler<BGSStarSystem>? VouchersClaimedEvent;
        #endregion

        #region LogProcessor Implementation
        public override string StoreName => "BGS Data";
        public override Dictionary<JournalTypeEnum, bool> EventsToParse
        {
            get => new()
            {
                { JournalTypeEnum.LoadGame,true},
                { JournalTypeEnum.Location,true},
                { JournalTypeEnum.FSDJump, true},
                { JournalTypeEnum.CarrierJump, true},
                { JournalTypeEnum.MissionAbandoned, true},
                { JournalTypeEnum.MissionAccepted, true},
                { JournalTypeEnum.MissionCompleted, true},
                { JournalTypeEnum.MissionFailed, true},
                { JournalTypeEnum.MissionRedirected, true},
                { JournalTypeEnum.Missions, true},
                { JournalTypeEnum.Docked, true},
                { JournalTypeEnum.Undocked, true},
                { JournalTypeEnum.RedeemVoucher, true},
            };
        }
        public override void ParseJournalEvent(JournalEntry evt)
        {
            if (EventsToParse.ContainsKey(evt.EventType) == false)
                return;

            switch (evt.EventData)
            {
                case LoadGameEvent.LoadGameEventArgs load:
                    odyssey = load.Odyssey;
                    break;
                case LocationEvent.LocationEventArgs location:
                    CurrentSystemAddress = location.SystemAddress;
                    CurrentSystemName = location.StarSystem;

                    if (string.IsNullOrEmpty(location.StationName) == false)
                    {
                        CurrentStationName = location.StationName;
                        CurrentMarketID = location.MarketID;
                    }

                    //Ignore systems with less than 2 factions
                    if (location.Factions is null || location.Factions.Count < 2)
                        break;

                    var system = new BGSStarSystem(location);

                    TryAddSystem(system);
                    break;
                case FSDJumpEvent.FSDJumpEventArgs fsdJump:
                    CurrentSystemAddress = fsdJump.SystemAddress;
                    CurrentSystemName = fsdJump.StarSystem;

                    if (fsdJump.Factions != null)
                    {
                        foreach (var faction in fsdJump.Factions)
                        {
                            factions.TryAdd(faction.Name, new(faction.Name, faction.Government, faction.Allegiance));
                        }
                    }

                    //Ignore systems with less than 2 factions
                    if (fsdJump.Factions is null || fsdJump.Factions.Count < 2)
                        break;
                    var nSystem = new BGSStarSystem(fsdJump);

                    TryAddSystem(nSystem);
                    break;
                case CarrierJumpEvent.CarrierJumpEventArgs carrierJump:
                    CurrentSystemAddress = carrierJump.SystemAddress;
                    CurrentSystemName = carrierJump.StarSystem;

                    if (string.IsNullOrEmpty(carrierJump.StationName) == false)
                    {
                        CurrentStationName = carrierJump.StationName;
                        CurrentMarketID = carrierJump.MarketID;
                    }

                    //Ignore systems with less than 2 factions
                    if (carrierJump.Factions is null || carrierJump.Factions.Count < 2)
                        break;

                    var cSystem = new BGSStarSystem(carrierJump);

                    TryAddSystem(cSystem);
                    break;
                case MissionAcceptedEvent.MissionAcceptedEventArgs accepted:
                    if (string.IsNullOrEmpty(CurrentStationName))
                    {
                        break;
                    }

                    var mission = new BGSMission(accepted,
                                                      CurrentSystemAddress,
                                                      CurrentSystemName,
                                                      CurrentMarketID,
                                                      CurrentStationName,
                                                      odyssey);

                    missions.Add(mission);
                    if (IsLive)
                        MissionAddedEvent?.Invoke(this, mission);
                    break;
                case MissionRedirectedEvent.MissionRedirectedEventArgs redirected:
                    var rMission = missions.FirstOrDefault(x => x.MissionID == redirected.MissionID);
                    if (rMission != null)
                    {
                        rMission.CurrentState = MissionState.Redirected;
                        if (IsLive)
                            MissionUpdatedEvent?.Invoke(this, rMission);
                    }
                    break;
                case MissionCompletedEvent.MissionCompletedEventArgs completed:
                    var cMission = missions.FirstOrDefault(x => x.MissionID == completed.MissionID);
                    if (cMission != null)
                    {
                        cMission.CurrentState = MissionState.Completed;
                        cMission.CompletionTime = completed.Timestamp;
                        cMission.Reward = completed.Reward == 0 ? -completed.Donated : completed.Reward;
                        cMission.ApplyFactionEffects(completed.FactionEffects);

                        if (IsLive)
                            MissionUpdatedEvent?.Invoke(this, cMission);
                    }
                    break;
                case MissionFailedEvent.MissionFailedEventArgs missionFailed:
                    var fMission = missions.FirstOrDefault(x => x.MissionID == missionFailed.MissionID);
                    if (fMission != null)
                    {
                        fMission.CurrentState = MissionState.Failed;
                        fMission.CompletionTime = missionFailed.Timestamp;
                        fMission.Reward = 0;
                        if (IsLive)
                            MissionUpdatedEvent?.Invoke(this, fMission);
                    }
                    break;
                case MissionAbandonedEvent.MissionAbandonedEventArgs abandoned:
                    var aMission = missions.FirstOrDefault(x => x.MissionID == abandoned.MissionID);
                    if (aMission != null)
                    {
                        aMission.CurrentState = MissionState.Abandoned;
                        aMission.CompletionTime = abandoned.Timestamp;
                        aMission.Reward = 0;
                        if (IsLive)
                            MissionUpdatedEvent?.Invoke(this, aMission);
                    }
                    break;
                case MissionsEvent.MissionsEventArgs missionsEvt:
                    var evtMissions = new List<Mission>();

                    evtMissions.AddRange(missionsEvt.Active);
                    evtMissions.AddRange(missionsEvt.Complete);
                    evtMissions.AddRange(missionsEvt.Failed);

                    if (evtMissions.Count == 0)
                        break;
                    var missionsToRemove = new List<BGSMission>();

                    foreach (var m in missions)
                    {
                        if (m.CurrentState > MissionState.Active || m.Odyssey != odyssey)
                            continue;

                        var mis = evtMissions.FirstOrDefault(x => x.MissionID == m.MissionID);

                        if (mis is null)
                        {
                            missionsToRemove.Add(m);
                        }
                    }

                    if (missionsToRemove.Count == 0)
                        break;

                    foreach (var ms in missionsToRemove)
                    {
                        missions.Remove(ms);
                    }
                    if (IsLive)
                        MissionsUpdatedEvent?.Invoke(this, EventArgs.Empty);
                    break;
                case DockedEvent.DockedEventArgs docked:
                    CurrentStationName = string.IsNullOrEmpty(docked.StationName_Localised) ? docked.StationName : docked.StationName_Localised;
                    CurrentSystemAddress = docked.SystemAddress;
                    CurrentSystemName = docked.StarSystem;
                    CurrentMarketID = docked.MarketID;
                    break;
                case UndockedEvent.UndockedEventArgs undocked:
                    CurrentStationName = string.Empty;
                    CurrentMarketID = 0;
                    break;
                case RedeemVoucherEvent.RedeemVoucherEventArgs redeemVoucher:
                    if (currentSystem == null)
                        break;
                    if (redeemVoucher.Factions == null || redeemVoucher.Factions.Count == 0)
                    {
                        if (string.IsNullOrEmpty(redeemVoucher.Faction))
                            break;

                        currentSystem
                            .VoucherClaims
                            .Add(new VoucherClaim(redeemVoucher.Type, redeemVoucher.Faction, redeemVoucher.Amount, redeemVoucher.Timestamp));

                        if (IsLive)
                            VouchersClaimedEvent?.Invoke(this, currentSystem);
                        break;
                    }

                    foreach (var faction in redeemVoucher.Factions)
                    {
                        if (string.IsNullOrEmpty(faction.Faction))
                            continue;
                        currentSystem
                            .VoucherClaims
                            .Add(new VoucherClaim(redeemVoucher.Type, faction, redeemVoucher.Timestamp));
                    }

                    if (IsLive)
                        VouchersClaimedEvent?.Invoke(this, currentSystem);
                    break;


            }
        }

        private void TryAddSystem(BGSStarSystem system)
        {
            
            if (systems.TryGetValue(system.Address, out var bgsSystem))
            {
                bgsSystem.AddTickData(system);
                currentSystem = bgsSystem;
                if (IsLive)
                {
                    //TODO fire event
                }
                return;
            }
           
            if (systems.TryAdd(system.Address, system))
            {
                currentSystem = system;
                if (IsLive)
                {
                    //TODO fire event
                }
            }
        }

        public override void ClearData()
        {
            missions.Clear();
            factions.Clear();
            systems.Clear();
            IsLive = false;
        }
        public override void Dispose()
        {
            this.journalManager.UnregisterLogProcessor(this);
        }
        #endregion

        private void OnNewTick(object? sender, EventArgs e)
        {
            tickContainer = new(tickDataStore.BGSTickData);
        }

        public Tuple<IEnumerable<BGSTickSystem>, IEnumerable<BGSMission>> GetTickInfo(string? id)
        {
            if(id == null)
                return Tuple.Create<IEnumerable<BGSTickSystem>, IEnumerable<BGSMission>>([], []);

            selectedTick = tickContainer.GetTickFromTo(id);

            var missions = this.missions.Where(x => selectedTick.TimeWithinTick(x.CompletionTime)
                                            && (x.CurrentState == MissionState.Completed || x.CurrentState == MissionState.Failed)).ToList();
            var systems = new List<BGSTickSystem>();

            foreach (var system in this.systems.Values)
            {
                var validSystem = system.GetBGSTickSystem(selectedTick);

                if (validSystem != null)
                {
                    systems.Add(validSystem);
                }
            }

            return Tuple.Create<IEnumerable<BGSTickSystem>, IEnumerable<BGSMission>>(systems, missions); 
        }
    }
}
