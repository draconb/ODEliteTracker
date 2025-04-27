using EliteJournalReader.Events;
using EliteJournalReader;
using ODEliteTracker.Models.Missions;
using ODJournalDatabase.JournalManagement;
using ODEliteTracker.Models.Galaxy;

namespace ODEliteTracker.Stores
{
    public sealed class BGSDataStore : LogProcessorBase
    {
        public BGSDataStore(IManageJournalEvents journalManager)
        {
            this.journalManager = journalManager;
            this.journalManager.RegisterLogProcessor(this);
        }

        private IManageJournalEvents journalManager;

        private long CurrentSystemAddress;
        private string CurrentSystemName = "Unknown";
        private long CurrentMarketID;
        private string CurrentStationName = "Unknown";
        private bool odyssey;

        private Dictionary<string, FactionData> factions = [];
        public override string StoreName => "BGS Data";
        public override Dictionary<JournalTypeEnum, bool> EventsToParse
        {
            get => new()
            {
                { JournalTypeEnum.LoadGame,true},
                { JournalTypeEnum.Location,true},
                { JournalTypeEnum.FSDJump, true},
                { JournalTypeEnum.MissionAbandoned, true},
                { JournalTypeEnum.MissionAccepted, true},
                { JournalTypeEnum.MissionCompleted, true},
                { JournalTypeEnum.MissionFailed, true},
                { JournalTypeEnum.MissionRedirected, true},
                { JournalTypeEnum.Missions, true},
                { JournalTypeEnum.Docked, true},
                { JournalTypeEnum.Undocked, true},
            };
        }

        public EventHandler<BGSMission>? OnMissionAddedEvent;
        public EventHandler<BGSMission>? OnMissionUpdatedEvent;
        public EventHandler<BGSMission>? OnCargoDepot;
        public EventHandler? OnMissionsUpdatedEvent;

        private readonly List<BGSMission> missions = [];
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
                        OnMissionAddedEvent?.Invoke(this, mission);
                    break;
                case MissionRedirectedEvent.MissionRedirectedEventArgs redirected:
                    var rMission = missions.FirstOrDefault(x => x.MissionID == redirected.MissionID);
                    if (rMission != null)
                    {
                        rMission.CurrentState = MissionState.Redirected;
                        if (IsLive)
                            OnMissionUpdatedEvent?.Invoke(this, rMission);
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
                            OnMissionUpdatedEvent?.Invoke(this, cMission);
                    }
                    break;
                case MissionFailedEvent.MissionFailedEventArgs missionFailed:
                    var fMission = missions.FirstOrDefault(x => x.MissionID == missionFailed.MissionID);
                    if (fMission != null)
                    {
                        fMission.CurrentState = MissionState.Failed;
                        fMission.Reward = 0;
                        if (IsLive)
                            OnMissionUpdatedEvent?.Invoke(this, fMission);
                    }
                    break;
                case MissionAbandonedEvent.MissionAbandonedEventArgs abandoned:
                    var aMission = missions.FirstOrDefault(x => x.MissionID == abandoned.MissionID);
                    if (aMission != null)
                    {
                        aMission.CurrentState = MissionState.Abandoned;
                        aMission.Reward = 0;
                        if (IsLive)
                            OnMissionUpdatedEvent?.Invoke(this, aMission);
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
                        if (m.CurrentState == MissionState.Completed || m.Odyssey != odyssey)
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
                        OnMissionsUpdatedEvent?.Invoke(this, EventArgs.Empty);
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

            }
        }

        public override void ClearData()
        {
            missions.Clear();
            factions.Clear();
            IsLive = false;
        }

        public override void Dispose()
        {
            this.journalManager.UnregisterLogProcessor(this);
        }
    }
}
