using EliteJournalReader;
using EliteJournalReader.Events;
using ODEliteTracker.Models.Galaxy;
using ODEliteTracker.Models.Market;
using ODEliteTracker.Models.Ship;
using ODJournalDatabase.JournalManagement;
using ODMVVM.Helpers;

namespace ODEliteTracker.Stores
{
    public sealed class SharedDataStore : LogProcessorBase
    {
        public SharedDataStore(IManageJournalEvents journalManager)
        {
            this.journalManager = journalManager;
            this.journalManager.RegisterLogProcessor(this);
        }

        #region Private fields
        private readonly IManageJournalEvents journalManager;
        #endregion

        #region Public Properties
        public override string StoreName => "Shared Data";
        public override Dictionary<JournalTypeEnum, bool> EventsToParse
        {
            get => new()
            {
                { JournalTypeEnum.Location, true },
                { JournalTypeEnum.FSDJump, true},
                { JournalTypeEnum.Docked, true},
                { JournalTypeEnum.Undocked, true},
                { JournalTypeEnum.ApproachBody, true},
                { JournalTypeEnum.Market, false },
                { JournalTypeEnum.Loadout, true },
                { JournalTypeEnum.Cargo, false },
            };
        }

        public StarSystem? CurrentSystem { get; private set; }
        public StationMarket? CurrentMarket { get; private set; }
        public string? CurrentBody_Station { get; private set; }
        public ShipInfo? CurrentShipInfo { get; private set; }
        public IEnumerable<ShipCargo>? CurrentShipCargo { get; private set; }
        #endregion

        #region Events
        public EventHandler<StarSystem?>? OnCurrentSystemChanged;
        public EventHandler<string?>? OnCurrentBody_Station;
        public EventHandler<StationMarket?>? OnMarketEvent;
        public EventHandler<ShipInfo?>? OnShipChangedEvent;
        public EventHandler<IEnumerable<ShipCargo>?>? OnShipCargoUpdatedEvent;
        #endregion

        public override void ClearData()
        {
            CurrentSystem = null;
            CurrentMarket = null;
            CurrentShipInfo = null;
            CurrentShipCargo = null;
            CurrentBody_Station = null;
            OnCurrentSystemChanged?.Invoke(this, null);
            OnMarketEvent?.Invoke(this, null);
            OnShipChangedEvent?.Invoke(this, null);
            OnShipCargoUpdatedEvent?.Invoke(this, null);
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
                    break;
                case FSDJumpEvent.FSDJumpEventArgs fsdJump:
                    UpdateCurrentSystem(new(fsdJump));
                    UpdateCurrentBody_Station(null);
                    break;
                case DockedEvent.DockedEventArgs docked:
                    UpdateCurrentBody_Station(string.IsNullOrEmpty(docked.StationName_Localised) ? docked.StationName : docked.StationName_Localised);
                    break;
                case UndockedEvent.UndockedEventArgs:
                    UpdateCurrentBody_Station(null);
                    break;
                case ApproachBodyEvent.ApproachBodyEventArgs approachBody:
                    UpdateCurrentBody_Station(approachBody.Body);
                    break;
                case MarketEvent.MarketEventArgs:
                    var market = journalManager.GetMarketInfo();

                    if(market != null)
                    {
                        CurrentMarket = new(market);
                        OnMarketEvent?.Invoke(this, CurrentMarket);
                    }
                    break;
                case LoadoutEvent.LoadoutEventArgs loadOut:
                    //Sometimes the ship name comes though as a string with a single space so we trim it
                    CurrentShipInfo = new ShipInfo(string.IsNullOrEmpty(loadOut.ShipName.Trim()) ? EliteHelpers.ConvertShipName(loadOut.Ship) : loadOut.ShipName, loadOut.ShipIdent, loadOut.CargoCapacity);
                    
                    if(IsLive)
                        OnShipChangedEvent?.Invoke(this, CurrentShipInfo);
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
                            OnShipCargoUpdatedEvent?.Invoke(this, CurrentShipCargo);
                        }
                    }
                    break;
            }
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

            OnCurrentSystemChanged?.Invoke(this, CurrentSystem);
        }

        private void UpdateCurrentBody_Station(string? text)
        {
            CurrentBody_Station = text;
            if (IsLive)
                OnCurrentBody_Station?.Invoke(this, CurrentBody_Station);
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
                    OnShipCargoUpdatedEvent?.Invoke(this, CurrentShipCargo);
                }
            }
        }
    }
}
