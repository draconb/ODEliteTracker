using ODEliteTracker.Models.BGS;
using ODEliteTracker.Models.Missions;
using ODEliteTracker.Stores;
using ODEliteTracker.ViewModels.ModelViews.BGS;
using ODMVVM.Commands;
using ODMVVM.Extensions;
using ODMVVM.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ODEliteTracker.ViewModels
{
    public sealed class BGSViewModel : ODViewModel
    {
        public BGSViewModel(BGSDataStore dataStore,
                            SharedDataStore sharedData,
                            SettingsStore settings)
        {
            this.dataStore = dataStore;
            this.sharedData = sharedData;
            this.settings = settings;
            this.dataStore.StoreLive += OnStoreLive;
            this.dataStore.MissionAddedEvent += OnMissionAdded;
            this.dataStore.MissionUpdatedEvent += OnMissionUpdated;
            this.dataStore.MissionsUpdatedEvent += OnMissionsUpdated;
            this.dataStore.SystemAdded += OnSystemAdded;
            this.dataStore.SystemUpdated += OnSystemUpdated;
            this.dataStore.VouchersClaimedEvent += OnVoucherClaimed;

            SetSelectedSystemCommand = new ODRelayCommand<BGSTickSystemVM>(OnSetSelectedSystem);
            if (dataStore.IsLive)
                OnStoreLive(null, true);
        }

        public override void Dispose()
        {
            this.dataStore.StoreLive -= OnStoreLive;
            this.dataStore.MissionAddedEvent -= OnMissionAdded;
            this.dataStore.MissionUpdatedEvent -= OnMissionUpdated;
            this.dataStore.MissionsUpdatedEvent -= OnMissionsUpdated;
            this.dataStore.SystemAdded -= OnSystemAdded;
            this.dataStore.SystemUpdated -= OnSystemUpdated;
            this.dataStore.VouchersClaimedEvent -= OnVoucherClaimed;
        }

        private void OnSetSelectedSystem(BGSTickSystemVM vM)
        {
            SelectedSystem = vM;
        }

        private readonly BGSDataStore dataStore;
        private readonly SharedDataStore sharedData;
        private readonly SettingsStore settings;

        public override bool IsLive => dataStore.IsLive;

        public ICommand SetSelectedSystemCommand { get; }

        public bool HideSystemsWithoutBGSData
        {
            get => settings.BGSViewSettings.HideSystemsWithoutData;
            set
            {
                settings.BGSViewSettings.HideSystemsWithoutData = value;
                OnPropertyChanged(nameof(HideSystemsWithoutBGSData));
                OnPropertyChanged(nameof(Systems));
            }
        }

        private BGSTickSystemVM? selectedSystem;
        public BGSTickSystemVM? SelectedSystem
        {
            get => selectedSystem;
            set
            {
                if (selectedSystem != null)
                {
                    selectedSystem.IsSelected = false;
                }
                selectedSystem = value;
                if (selectedSystem != null)
                {
                    selectedSystem.IsSelected = true;
                }
                OnPropertyChanged(nameof(SelectedSystem));
            }
        }
        private ObservableCollection<BGSTickSystemVM> systems { get; set; } = [];

        public IEnumerable<BGSTickSystemVM> Systems
        {
            get
            {
                if (HideSystemsWithoutBGSData)
                    return systems.Where(x => x.Address == sharedData.CurrentSystem?.Address || x.HasData).OrderBy(x => x.Name);
                return systems;
            }
        }

        public ObservableCollection<TickDataVM> Ticks { get; set; } = [];

        private TickDataVM? selectedTick;
        public TickDataVM? SelectedTick
        {
            get => selectedTick;
            set
            {
                selectedTick = value;
                PopulateSystems();
                OnPropertyChanged(nameof(SelectedTick));
            }
        }

        private void OnStoreLive(object? sender, bool e)
        {
            if (e)
            {

                var ticks = dataStore.TickData.OrderByDescending(x => x.Time).Select(x => new TickDataVM(x));
                Ticks.AddRange(ticks);

                SelectedTick = Ticks.FirstOrDefault(x => string.Equals(x.ID, dataStore.SelectedTick?.ID)) ?? Ticks.FirstOrDefault();
                PopulateSystems();

                OnPropertyChanged(nameof(Ticks));
                OnModelLive(true);
            }
        }

        private void OnVoucherClaimed(object? sender, BGSStarSystem e)
        {
            //TODO Implement updating properties
            PopulateSystems(e);
        }

        private void OnSystemUpdated(object? sender, BGSStarSystem e)
        {
            //TODO Implement updating properties
            PopulateSystems(e);
        }

        private void OnSystemAdded(object? sender, BGSStarSystem e)
        {
            //TODO Implement updating properties
            PopulateSystems(e);
        }

        private void OnMissionsUpdated(object? sender, EventArgs e)
        {
            //TODO Implement updating properties
            PopulateSystems();
        }

        private void OnMissionUpdated(object? sender, BGSMission e)
        {
            //TODO Implement updating properties
            PopulateSystems();
        }

        private void OnMissionAdded(object? sender, BGSMission e)
        {
            //TODO Implement updating properties
            PopulateSystems();
        }

        private void PopulateSystems(BGSStarSystem? system = null)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var tickData = dataStore.GetTickInfo(SelectedTick?.ID);

                var systems = tickData.Item1.OrderBy(x => x.Name).Select(x => new BGSTickSystemVM(x));

                this.systems.ClearCollection();
                this.systems.AddRange(systems);
                SelectedSystem = system == null ? this.systems.FirstOrDefault() : this.systems.FirstOrDefault(x => x.Address == system.Address);
                foreach (var mission in tickData.Item2)
                {
                    switch (mission.CurrentState)
                    {
                        case Models.Missions.MissionState.Completed:
                            if (mission.FactionEffects is null || mission.FactionEffects.Count == 0)
                            {
                                continue;
                            }

                            foreach (var effect in mission.FactionEffects)
                            {
                                foreach (var inf in effect.Influence)
                                {
                                    var faction = Systems.FirstOrDefault(x => x.Address == inf.SystemAddress)?
                                        .Factions.FirstOrDefault(x => string.Equals(x.Name, effect.FactionName));

                                    if (faction != null)
                                    {
                                        faction.InfPlus += inf.Influence;
                                    }
                                }
                            }
                            break;
                        case Models.Missions.MissionState.Failed:
                            var fction = Systems.FirstOrDefault(x => x.Address == mission.OriginSystemAddress)?
                                       .Factions.FirstOrDefault(x => string.Equals(x.Name, mission.IssuingFaction));

                            if (fction != null)
                            {
                                fction.Failed++;
                            }
                            break;
                    }
                }

                OnPropertyChanged(nameof(Systems));
            });
        }
    }
}
