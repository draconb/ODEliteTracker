using EliteJournalReader;
using EliteJournalReader.Events;
using NetTopologySuite.Geometries;
using ODEliteTracker.Helpers;
using ODEliteTracker.Models.Galaxy;
using ODEliteTracker.Models.Market;
using ODEliteTracker.Models.Ship;
using ODEliteTracker.Notifications;
using ODEliteTracker.Notifications.ScanNotification;
using ODEliteTracker.Services;
using ODJournalDatabase.JournalManagement;
using ODMVVM.Helpers;
using System.Xml.Linq;

namespace ODEliteTracker.Stores
{
    public sealed class SharedDataStore : LogProcessorBase
    {
        public SharedDataStore(IManageJournalEvents journalManager,
                               NotificationService notificationService)
        {
            this.journalManager = journalManager;
            this.notificationService = notificationService;
            this.journalManager.RegisterLogProcessor(this);
        }

        #region Private fields
        private readonly IManageJournalEvents journalManager;
        private readonly NotificationService notificationService;
        private Dictionary<string, FactionData> factions = [];
        private string? lastShipTarget;
        private string? commanderPower;
        #endregion

        #region Public Properties
        public override string StoreName => "Shared Data";
        public override Dictionary<JournalTypeEnum, bool> EventsToParse
        {
            get => new()
            {
                { JournalTypeEnum.Powerplay, true },
                { JournalTypeEnum.Location, true },
                { JournalTypeEnum.FSDJump, true},
                { JournalTypeEnum.CarrierJump, true},
                { JournalTypeEnum.Docked, true},
                { JournalTypeEnum.Undocked, true},
                { JournalTypeEnum.ApproachBody, true},
                { JournalTypeEnum.Market, false },
                { JournalTypeEnum.Loadout, true },
                { JournalTypeEnum.Cargo, false },
                { JournalTypeEnum.ShipTargeted, false },
            };
        }
        public Dictionary<string, FactionData> Factions => factions;
        public StarSystem? CurrentSystem { get; private set; }
        public StationMarket? CurrentMarket { get; private set; }
        public string? CurrentBody_Station { get; private set; }
        public ShipInfo? CurrentShipInfo { get; private set; }
        public IEnumerable<ShipCargo>? CurrentShipCargo { get; private set; }
        #endregion

        #region Events
        public EventHandler<StarSystem?>? CurrentSystemChanged;
        public EventHandler<string?>? CurrentBody_StationChanged;
        public EventHandler<StationMarket?>? MarketEvent;
        public EventHandler<ShipInfo?>? ShipChangedEvent;
        public EventHandler<IEnumerable<ShipCargo>?>? ShipCargoUpdatedEvent;
        #endregion

        public override void ClearData()
        {
            CurrentSystem = null;
            CurrentMarket = null;
            CurrentShipInfo = null;
            CurrentShipCargo = null;
            CurrentBody_Station = null;
            CurrentSystemChanged?.Invoke(this, null);
            MarketEvent?.Invoke(this, null);
            ShipChangedEvent?.Invoke(this, null);
            ShipCargoUpdatedEvent?.Invoke(this, null);
            IsLive = false;
        }

        public override void Dispose()
        {
            journalManager.UnregisterLogProcessor(this);
        }

        public override DateTime GetJournalAge(DateTime defaultAge)
        {
            return defaultAge;
        }

        public override Task ParseHistoryStream(JournalEntry entry)
        {
            ParseJournalEvent(entry);
            return Task.CompletedTask;
        }

        public override void ParseJournalEvent(JournalEntry evt)
        {
            if (EventsToParse.ContainsKey(evt.EventType) == false)
                return;

            switch (evt.EventData)
            {
                case PowerplayEvent.PowerplayEventArgs powerPlay:
                    commanderPower = powerPlay.Power;
                    break;
                case LocationEvent.LocationEventArgs location:
                    UpdateCurrentSystem(new(location));
                    string? bodyStation = null;
                    if (string.IsNullOrEmpty(location.Body) == false)
                    {
                        bodyStation = location.Body;
                    }
                    if (string.IsNullOrEmpty(location.StationName) == false)
                    {
                        bodyStation = location.StationName;
                    }
                    UpdateCurrentBody_Station(bodyStation);
                    AddFactions(location.Factions);

                    if(location.Population == 0 || location.SystemFaction is null || location.Factions is null || location.Factions.Count == 0)
                    {
                        SystemNotification(location.StarSystem,
                        [
                            "Unpopulated",
                            location.SystemSecurity_Localised,
                        ]);
                        break;
                    }

                    SystemNotification(location.StarSystem,
                        [
                            location.SystemFaction.Name,
                            $"Population : {EliteHelpers.FormatNumber(location.Population ?? 0)}",
                            location.SystemAllegiance,
                            location.SystemFaction.FactionState,
                            location.SystemSecurity_Localised,
                            EliteHelpers.FactionReputationToString(location.Factions.FirstOrDefault(x => string.Equals(x.Name, location.SystemFaction.Name))?.MyReputation)
                        ]);
                    break;
                case FSDJumpEvent.FSDJumpEventArgs fsdJump:
                    UpdateCurrentSystem(new(fsdJump));
                    UpdateCurrentBody_Station(null);
                    AddFactions(fsdJump.Factions);
                    if (fsdJump.Population == 0 || fsdJump.SystemFaction is null || fsdJump.Factions is null || fsdJump.Factions.Count == 0)
                    {
                        SystemNotification(fsdJump.StarSystem,
                        [
                            "Unpopulated",
                            fsdJump.SystemSecurity_Localised,
                        ]);
                        break;
                    }
                    SystemNotification(fsdJump.StarSystem,
                    [
                            fsdJump.SystemFaction.Name,
                            $"Population : {EliteHelpers.FormatNumber(fsdJump.Population)}",
                            fsdJump.SystemAllegiance,
                            fsdJump.SystemFaction.FactionState,
                            fsdJump.SystemSecurity_Localised,
                            EliteHelpers.FactionReputationToString(fsdJump.Factions.FirstOrDefault(x => string.Equals(x.Name, fsdJump.SystemFaction.Name))?.MyReputation)
                        ]);
                    break;
                case CarrierJumpEvent.CarrierJumpEventArgs carrierJump:
                    UpdateCurrentSystem(new(carrierJump));
                    string? bodyStn = null;
                    if (string.IsNullOrEmpty(carrierJump.Body) == false)
                    {
                        bodyStn = carrierJump.Body;
                    }
                    if (string.IsNullOrEmpty(carrierJump.StationName) == false)
                    {
                        bodyStn = carrierJump.StationName;
                    }                    
                    UpdateCurrentBody_Station(bodyStn);  
                    AddFactions(carrierJump.Factions);

                    if (carrierJump.Population == 0 || carrierJump.SystemFaction is null || carrierJump.Factions is null || carrierJump.Factions.Count == 0)
                    {
                        SystemNotification(carrierJump.StarSystem,
                        [
                            "Unpopulated",
                            carrierJump.SystemSecurity_Localised,
                        ]);
                        break;
                    }

                    SystemNotification(carrierJump.StarSystem,
                        [
                            carrierJump.SystemFaction.Name,
                            $"Population : {EliteHelpers.FormatNumber(carrierJump.Population ?? 0)}",
                            carrierJump.SystemAllegiance,
                            carrierJump.SystemFaction.FactionState,
                            carrierJump.SystemSecurity_Localised,
                            EliteHelpers.FactionReputationToString(carrierJump.Factions.FirstOrDefault(x => string.Equals(x.Name, carrierJump.SystemFaction.Name))?.MyReputation)
                        ]);
                    break;
                case DockedEvent.DockedEventArgs docked:
                    UpdateCurrentBody_Station(string.IsNullOrEmpty(docked.StationName_Localised) ? docked.StationName : docked.StationName_Localised);

                    if (IsLive == false)
                        break;

                    var args = new NotificationArgs(docked.StationName_Localised ?? docked.StationName,
                        [
                            $"{EliteJournalReaderHelpers.StationTypeText(docked.StationType)}",
                            $"{docked.StationFaction.Name}",
                            $"{docked.StationGovernment_Localised ?? docked.StationGovernment}",
                            $"{docked.StationEconomy_Localised ?? docked.StationEconomy}",
                            $"{docked.LandingPads.LandPadText()}"
                        ],
                        NotificationType.Station);

                    notificationService.ShowBasicNotification(args);
                    break;
                case UndockedEvent.UndockedEventArgs:
                    UpdateCurrentBody_Station(null);
                    break;
                case ApproachBodyEvent.ApproachBodyEventArgs approachBody:
                    UpdateCurrentBody_Station(approachBody.Body);
                    break;
                case EliteJournalReader.Events.MarketEvent.MarketEventArgs:
                    var market = journalManager.GetMarketInfo();

                    if(market != null)
                    {
                        CurrentMarket = new(market);
                        MarketEvent?.Invoke(this, CurrentMarket);
                    }
                    break;
                case LoadoutEvent.LoadoutEventArgs loadOut:
                    //Sometimes the ship name comes though as a string with a single space so we trim it
                    CurrentShipInfo = new ShipInfo(string.IsNullOrEmpty(loadOut.ShipName.Trim()) ? EliteHelpers.ConvertShipName(loadOut.Ship) : loadOut.ShipName, loadOut.ShipIdent, loadOut.CargoCapacity);
                    
                    if(IsLive)
                        ShipChangedEvent?.Invoke(this, CurrentShipInfo);
                    break;
                case CargoEvent.CargoEventArgs:
                    var cargo = journalManager.GetCargo();

                    if (cargo != null && cargo.Vessel.Equals("Ship", StringComparison.OrdinalIgnoreCase))
                    {
                        CurrentShipCargo = cargo?.Inventory.Select(x =>
                        {
                            return new ShipCargo((string.IsNullOrEmpty(x.Name_Localised) ? x.Name : x.Name_Localised).ToTitleCase(), x.Count);
                        });

                        if (CurrentShipCargo != null)
                        {
                            ShipCargoUpdatedEvent?.Invoke(this, CurrentShipCargo);
                        }
                    }
                    break;
                case ShipTargetedEvent.ShipTargetedEventArgs shipTargeted:

                    if (shipTargeted.ScanStage != 3)
                        break;

                    var pilotName = shipTargeted.PilotName_Localised ?? shipTargeted.PilotName;

                    if (string.IsNullOrEmpty(pilotName) || string.Equals(pilotName, lastShipTarget))
                    {
                        return;
                    }

                    lastShipTarget = pilotName;

                    if (string.IsNullOrEmpty(lastShipTarget) || string.IsNullOrEmpty(shipTargeted.Power) && shipTargeted.Bounty <= 0)
                    {
                        break;
                    }

                    var targetType = TargetType.None;

                    if(string.IsNullOrEmpty(shipTargeted.Power) == false && string.Equals(shipTargeted.Power, commanderPower) == false)
                    {
                        targetType |= TargetType.Enemy;
                    }

                    if (shipTargeted.Bounty > 0)
                    {
                        targetType |= TargetType.Wanted;                    
                    }

                    //Shouldn't reach this but just in case
                    if (targetType == TargetType.None)
                    {
                        break;
                    }

                    notificationService.ShowShipTargetedNotification(pilotName, EliteHelpers.ConvertShipName(shipTargeted.Ship), targetType, shipTargeted.Bounty, shipTargeted.Faction, shipTargeted.Power);
                    break;

            }
        }

        private void SystemNotification(string name, string[] fields)
        {
            if (IsLive == false)
                return;

            var args = new NotificationArgs(name, fields, NotificationType.System);
            notificationService.ShowBasicNotification(args);
        }

        private void UpdateCurrentSystem(StarSystem starSystem)
        {
            if (CurrentSystem != null && starSystem.Address == CurrentSystem.Address)
            {
                //Nothing to do here
                return;
            }

            CurrentSystem = starSystem;

            if (IsLive == false)
            {
                //No need to trigger events if parsing history
                return;
            }

            CurrentSystemChanged?.Invoke(this, CurrentSystem);
        }

        private void UpdateCurrentBody_Station(string? text)
        {
            CurrentBody_Station = text;
            if (IsLive)
                CurrentBody_StationChanged?.Invoke(this, CurrentBody_Station);
        }

        private void AddFactions(IEnumerable<Faction>? factions)
        {
            if (factions != null && factions.Any())
            {
                foreach (var faction in factions)
                {
                    this.factions.TryAdd(faction.Name, new(faction.Name, faction.Government, faction.Allegiance));
                }
            }
        }

        public override void RunAfterParsingHistory()
        {
            IsLive = true;
            var cargo = journalManager.GetCargo();

            if (cargo != null && cargo.Vessel.Equals("Ship", StringComparison.OrdinalIgnoreCase))
            {
                CurrentShipCargo = cargo?.Inventory.Select(x => new ShipCargo((string.IsNullOrEmpty(x.Name_Localised) ? x.Name : x.Name_Localised).ToTitleCase(), x.Count));

                if (CurrentShipCargo != null)
                {
                    ShipCargoUpdatedEvent?.Invoke(this, CurrentShipCargo);
                }
            }
        }
    }
}
