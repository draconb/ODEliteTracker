using ODEliteTracker.Models;
using ODEliteTracker.Models.Colonisation;
using ODEliteTracker.Models.Market;
using ODEliteTracker.Models.Ship;
using ODEliteTracker.Stores;
using ODEliteTracker.ViewModels.ModelViews;
using ODEliteTracker.ViewModels.ModelViews.Colonisation;
using ODEliteTracker.ViewModels.ModelViews.Market;
using ODMVVM.Commands;
using ODMVVM.Extensions;
using ODMVVM.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ODEliteTracker.ViewModels
{
    public sealed class ColonisationViewModel : ODViewModel
    {
        public ColonisationViewModel(ColonisationStore colonisationStore,
                                     SharedDataStore sharedDataStore,
                                     SettingsStore settings)
        {
            this.colonisationStore = colonisationStore;
            this.sharedData = sharedDataStore;
            this.settings = settings;
            this.colonisationStore.StoreLive += OnStoreLive;
            this.colonisationStore.DepotUpdated += OnDepotUpdated;
            this.colonisationStore.NewDepot += OnNewDepot;
            this.colonisationStore.NewCommanderSystem += OnNewCommanderSystem;
            this.colonisationStore.ReleaseCommanderSystem += OnReleaseCommandSystem;

            this.sharedData.MarketEvent += OnMarketEvent;
            this.sharedData.ShipChangedEvent += OnShipChanged;
            this.sharedData.ShipCargoUpdatedEvent += OnCargoUpdated;

            if (this.sharedData.IsLive)
            {
                if (this.sharedData.CurrentShipInfo != null)
                    OnShipChanged(null, this.sharedData.CurrentShipInfo);
                if(this.sharedData.CurrentShipCargo != null)
                    OnCargoUpdated(null, this.sharedData.CurrentShipCargo);
            }

            SetSelectedDepotCommand = new ODRelayCommand<ConstructionDepotVM?>(SetSelectedDepot);
            SetSelectedCommanderSystemCommand = new ODRelayCommand<CommanderSystemVM?>(SetSelectedCommanderSystem);
            SetActiveStateCommand = new ODRelayCommand<ConstructionDepotVM>(SetDepotActiveState);
            Depots.CollectionChanged += Depots_CollectionChanged;

            if (colonisationStore.IsLive)
            {
                OnStoreLive(null, true);
            }
        }

        public override void Dispose()
        {
            this.colonisationStore.StoreLive -= OnStoreLive;
            this.colonisationStore.DepotUpdated -= OnDepotUpdated;
            this.colonisationStore.NewDepot -= OnNewDepot;
            this.colonisationStore.NewCommanderSystem -= OnNewCommanderSystem;
            this.colonisationStore.ReleaseCommanderSystem -= OnReleaseCommandSystem;

            this.sharedData.MarketEvent -= OnMarketEvent;
            this.sharedData.ShipChangedEvent -= OnShipChanged;
            this.sharedData.ShipCargoUpdatedEvent -= OnCargoUpdated;
        }

        #region Private fields
        private readonly ColonisationStore colonisationStore;
        private readonly SharedDataStore sharedData;
        private readonly SettingsStore settings;
        #endregion

        #region Commands
        public ICommand SetSelectedDepotCommand { get; }
        public ICommand SetActiveStateCommand { get; }
        public ICommand SetSelectedCommanderSystemCommand { get; }
        #endregion

        #region Public properties
        public override bool IsLive { get => colonisationStore.IsLive; }

        public string ActiveButtonText
        {
            get
            {
                if (selectedDepot == null)
                    return string.Empty;

                return selectedDepot.Inactive ? "Set Active" : "Set Inactive";
            }
        }
        public CommoditySorting CommoditySorting
        {
            get => settings.ColonisationCommoditySorting;
            set
            {
                settings.ColonisationCommoditySorting = value;
                OnPropertyChanged(nameof(SelectedDepotResources));
                OnPropertyChanged(nameof(CurrentMarketItems));
            }
        }
        public ObservableCollection<ConstructionDepotVM> Depots { get; } = [];
        public IEnumerable<ConstructionDepotVM> ActiveDepots => Depots.Where(x => x.Inactive == false);
        public IEnumerable<ConstructionDepotVM> InactiveDepots => Depots.Where(x => x.Inactive == true);
        public IEnumerable<ConstructionResourceVM>? SelectedDepotResources
        {
            get
            {
                if (SelectedDepot == null)
                    return null;

                return CommoditySorting switch
                {
                    CommoditySorting.ShowAll => SelectedDepot.Resources,
                    CommoditySorting.Category => SelectedDepot.Resources.OrderBy(x => x.Category).ThenBy(x => x.LocalName).Where(x => x.RemainingCount > 0),
                    _ => SelectedDepot.Resources.OrderBy(x => x.LocalName).Where(x => x.RemainingCount > 0),
                };
            }
        }

        private ConstructionDepotVM? selectedDepot;
        public ConstructionDepotVM? SelectedDepot
        {
            get => selectedDepot;
            set
            {
                if(selectedDepot == value) 
                    return;
                if (selectedDepot != null)
                    selectedDepot.IsSelected = false;
                selectedDepot = value;
                if (selectedDepot != null)
                    selectedDepot.IsSelected = true;
                CheckMarket();
                OnPropertyChanged(nameof(SelectedDepot));
                OnPropertyChanged(nameof(ActiveButtonText));
                OnPropertyChanged(nameof(SelectedDepotResources));
            }
        }
        public ObservableCollection<CommanderSystemVM> CommanderSystems { get; } = [];

        private CommanderSystemVM? selectedCommanderSystem;
        public CommanderSystemVM? SelectedCommanderSystem
        {
            get => selectedCommanderSystem;
            set
            {
                selectedCommanderSystem = value;
                OnPropertyChanged(nameof(SelectedCommanderSystem));
            }
        }
        public string? CurrentSystem { get; set; }

        private StationMarketVM? currentMarket;
        public StationMarketVM? CurrentMarket
        {
            get => currentMarket;
            set
            {
                currentMarket = value;
                OnPropertyChanged(nameof(CurrentMarket));
                OnPropertyChanged(nameof(CurrentMarketItems));
            }
        }

        public IEnumerable<StationCommodityVM>? CurrentMarketItems
        {
            get
            {
                if (CurrentMarket is null)
                    return null;
                return CommoditySorting switch
                {
                    CommoditySorting.Category => CurrentMarket.ItemsForSale.Where(x => x.RequiredResource).OrderBy(x => x.Category_Localised).ThenBy(x => x.Name_Localised),
                    CommoditySorting.Name => CurrentMarket.ItemsForSale.Where(x => x.RequiredResource).OrderBy(x => x.Name_Localised),
                    _ => CurrentMarket.ItemsForSale.OrderBy(x => x.Name_Localised),
                };
            }
        }

        private ShipInfoVM? currentShip;
        public ShipInfoVM? CurrentShip
        {
            get => currentShip;
            set
            {
                currentShip = value;
                OnPropertyChanged(nameof(CurrentShip));
            }
        }
        #endregion

        private void Depots_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ActiveDepots));
            OnPropertyChanged(nameof(InactiveDepots));
        }

        private void SetSelectedDepot(ConstructionDepotVM? vM)
        {
            SelectedDepot = vM;
        }

        private void SetDepotActiveState(ConstructionDepotVM vM)
        {
            var inactive = vM.Inactive;
            vM.Inactive = !inactive;
            colonisationStore.SetDepotActiveState(vM);
            OnPropertyChanged(nameof(ActiveDepots));
            OnPropertyChanged(nameof(InactiveDepots));
            OnPropertyChanged(nameof(ActiveButtonText));
        }

        private void OnNewDepot(object? sender, ConstructionDepot? e)
        {
            if (e == null)
                return;

            var newDepot = new ConstructionDepotVM(e);
            Depots.AddItem(newDepot);
            SelectedDepot = newDepot;

            var cmdrSystem = CommanderSystems.FirstOrDefault(x => x.SystemAddress == newDepot.SystemAddress);
            cmdrSystem?.Depots.Add(newDepot);

            OnPropertyChanged(nameof(Depots));
        }

        private void OnDepotUpdated(object? sender, ConstructionDepot? e)
        {
            if (e == null)
                return;
            var known = Depots.FirstOrDefault(x => x.MarketID == e.MarketID);

            if (known != null)
            {
                known.Update(e);
                SelectedDepot = known;
                return;
            }

            //Must be new or one we missed?
            OnNewDepot(sender, e);
        }

        private void OnStoreLive(object? sender, bool e)
        {
            if (e == false)
                return;

            if(colonisationStore.CommanderSystems.Any())
            {
                foreach (var system in colonisationStore.CommanderSystems)
                {
                    var newSystem = new CommanderSystemVM(system.SystemAddress, system.SystemName);
                    CommanderSystems.AddItem(newSystem);
                }
            }

            if (colonisationStore.Depots.Any())
            {
   
                foreach (var depot in colonisationStore.Depots)
                {
                    var newDepot = new ConstructionDepotVM(depot);
                    Depots.AddItem(newDepot);
                    var cmdrSystem = CommanderSystems.FirstOrDefault(x => x.SystemAddress == newDepot.SystemAddress);
                    cmdrSystem?.Depots.AddItem(newDepot);
                }
                OnPropertyChanged(nameof(ActiveDepots));
                OnPropertyChanged(nameof(InactiveDepots));
            }
            SelectedDepot = Depots.LastOrDefault();
            SelectedCommanderSystem = CommanderSystems.LastOrDefault();
            OnModelLive(true);
        }

        private void OnMarketEvent(object? sender, StationMarket? e)
        {
            CheckMarket();
        }

        private void CheckMarket()
        {
            if(selectedDepot == null || sharedData.CurrentMarket == null)
                return; 

            var market = StationMarketVM.CreateEmptyItemMarket(sharedData.CurrentMarket);

            foreach(var item in sharedData.CurrentMarket.ItemsForSale)
            {
                var required = selectedDepot.FilteredResources.FirstOrDefault(x => x.FDEVName == item.Name);

                var commodity = new StationCommodityVM(item, required != null)
                {
                    Required = required?.RemainingCount ?? 0
                };

                market.ItemsForSale.Add(commodity);
            }            

            CurrentMarket = market;
        }

        private void OnShipChanged(object? sender, ShipInfo? e)
        {
            CurrentShip = e == null ? null : new(e);
        }

        private void OnCargoUpdated(object? sender, IEnumerable<ShipCargo>? e)
        {
            CurrentShip?.OnCargoUpdated(e);
        }

        private void OnReleaseCommandSystem(object? sender, CommanderSystem e)
        {
            var cmdrSystem = CommanderSystems.FirstOrDefault(x => x.SystemAddress == e.SystemAddress);

            if (cmdrSystem != null)
            {
                CommanderSystems.RemoveItem(cmdrSystem);
                SelectedCommanderSystem = CommanderSystems.LastOrDefault();
            }
        }

        private void OnNewCommanderSystem(object? sender, CommanderSystem e)
        {
            var cmdrSystem = CommanderSystems.FirstOrDefault(x => x.SystemAddress == e.SystemAddress);

            if (cmdrSystem == null)
            {
                cmdrSystem = new(e.SystemAddress, e.SystemName);
                CommanderSystems.AddItem(cmdrSystem);
                SelectedCommanderSystem = cmdrSystem;
            }
        }
        private void SetSelectedCommanderSystem(CommanderSystemVM? vM)
        {
            SelectedCommanderSystem = vM;
        }
    }
}
