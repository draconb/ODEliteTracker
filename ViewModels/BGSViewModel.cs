using ODEliteTracker.Models.BGS;
using ODEliteTracker.Models.Missions;
using ODEliteTracker.Stores;
using ODEliteTracker.ViewModels.ModelViews.BGS;
using ODMVVM.Commands;
using ODMVVM.Extensions;
using ODMVVM.Services.MessageBox;
using ODMVVM.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ODEliteTracker.ViewModels
{
    public sealed class BGSViewModel : ODViewModel
    {
        public BGSViewModel(BGSDataStore dataStore,
                            SettingsStore settings)
        {
            this.dataStore = dataStore;
            this.settings = settings;
            this.dataStore.StoreLive += OnStoreLive;
            this.dataStore.MissionAddedEvent += OnMissionAdded;
            this.dataStore.MissionUpdatedEvent += OnMissionUpdated;
            this.dataStore.MissionsUpdatedEvent += OnMissionsUpdated;
            this.dataStore.SystemAdded += OnSystemAdded;
            this.dataStore.SystemUpdated += OnSystemUpdated;
            this.dataStore.VouchersClaimedEvent += OnVoucherClaimed;
            this.dataStore.OnNewTickDetected += OnNewTick;

            SetSelectedSystemCommand = new ODRelayCommand<BGSTickSystemVM>(OnSetSelectedSystem);
            AddNewTickCommand = new ODAsyncRelayCommand<Window?>(OnAddNewTick);
            DeletedTickCommand = new ODAsyncRelayCommand(OnDeleteTick, () => SelectedTick?.ManualTick == true);
            CreateDiscordPostCommand = new ODRelayCommand(OnCreateDiscordPost);
            OpenInaraCommand = new ODRelayCommand(OnOpenInara);


            missionExpiryUpdateTimer = new Timer(OnUpdateExpiry, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            if (dataStore.IsLive)
                OnStoreLive(null, true);
        }

        private void OnOpenInara(object? obj)
        {
            if (selectedSystem == null)
                return;

            ODMVVM.Helpers.OperatingSystem.OpenUrl($"https://inara.cz/galaxy-starsystem/?search={selectedSystem.NonUpperName.Replace(' ', '+')}");
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
            this.dataStore.OnNewTickDetected -= OnNewTick;
            missionExpiryUpdateTimer.Dispose();
        }

        private void OnSetSelectedSystem(BGSTickSystemVM vM)
        {
            SelectedSystem = vM;
        }

        private void OnNewTick(object? sender, EventArgs e)
        {
            var ticks = dataStore.TickData.OrderByDescending(x => x.Time).Select(x => new TickDataVM(x));
            Ticks.ClearCollection();
            Ticks.AddRange(ticks);

            SelectedTick = Ticks.FirstOrDefault();

            PopulateSystems();
            PopulateMissions();
            OnPropertyChanged(nameof(Ticks));
        }

        private async Task OnDeleteTick()
        {
            if (SelectedTick == null)
                return;

            await dataStore.DeleteTick(SelectedTick.ID);

            var ticks = dataStore.TickData.OrderByDescending(x => x.Time).Select(x => new TickDataVM(x));
            Ticks.ClearCollection();
            Ticks.AddRange(ticks);

            SelectedTick = Ticks.FirstOrDefault();

            PopulateSystems();
            PopulateMissions();
            OnPropertyChanged(nameof(Ticks));
        }

        private async Task OnAddNewTick(Window? window)
        {
            var time = ODDialogService.ShowDateTimeSelector(window, "Add Tick", DateTime.UtcNow.AddYears(1286));

            if (time.Result == true)
            {
                var tick = await dataStore.AddTick(time.Time.AddYears(-1286));

                var ticks = dataStore.TickData.OrderByDescending(x => x.Time).Select(x => new TickDataVM(x));
                Ticks.ClearCollection();
                Ticks.AddRange(ticks);

                SelectedTick = Ticks.FirstOrDefault(x => string.Equals(x.ID, tick.Id)) ?? Ticks.FirstOrDefault();

                PopulateSystems();
                PopulateMissions();
                OnPropertyChanged(nameof(Ticks));
            }
        }

        private readonly Timer missionExpiryUpdateTimer;
        private readonly BGSDataStore dataStore;
        private readonly SettingsStore settings;

        public override bool IsLive => dataStore.IsLive;

        public ICommand SetSelectedSystemCommand { get; }
        public ICommand AddNewTickCommand { get; }
        public ICommand DeletedTickCommand { get; }
        public ICommand CreateDiscordPostCommand { get; }
        public ICommand OpenInaraCommand { get; }
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

        public int SelectedTab
        {
            get => settings.BGSViewSettings.SelectedTab;
            set
            {
                settings.BGSViewSettings.SelectedTab = value;
                OnPropertyChanged(nameof(SelectedTab));
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
                    return systems.Where(x => x.Address == dataStore.CurrentSystem?.Address || x.HasData).OrderBy(x => x.Name);
                return systems;
            }
        }
        public IEnumerable<MegaShipScanVM> MegaShipScans { get; private set; } = [];

        public ObservableCollection<TickDataVM> Ticks { get; set; } = [];

        public ObservableCollection<BGSMissionVM> Missions { get; set; } = [];

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

        private string discordButtonText = "Create Post";
        public string DiscordButtonText
        {
            get => discordButtonText;
            set
            {
                discordButtonText = value;
                OnPropertyChanged(nameof(DiscordButtonText));
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
                PopulateMegaships();
                PopulateMissions();
                OnPropertyChanged(nameof(Ticks));
                missionExpiryUpdateTimer.Change(new TimeSpan(0, 1, 0), new TimeSpan(0, 1, 0));
                OnModelLive(true);
            }
        }

        private void OnCreateDiscordPost(object? obj)
        {
            if (selectedTick is null)
            {
                return;
            }

            if(Helpers.DiscordPostCreator.CreateBGSPost(systems, selectedTick))
            {
                DiscordButtonText = "Post Created";
                Task.Delay(4000).ContinueWith(e => { DiscordButtonText = "Create Post"; });
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
            PopulateMissions();
        }

        private void OnMissionUpdated(object? sender, BGSMission e)
        {
            //TODO Implement updating properties
            PopulateSystems();
            PopulateMissions();
        }

        private void OnMissionAdded(object? sender, BGSMission e)
        {
            //TODO Implement updating properties
            PopulateSystems();
            PopulateMissions();
        }

        private void OnUpdateExpiry(object? state)
        {
            if (Missions.Count == 0)
                return;

            lock (Missions)
            {
                foreach (var mission in Missions)
                {
                    mission.UpdateExpiry();
                }
            }
        }

        private void PopulateSystems(BGSStarSystem? system = null)
        {
            var tickData = dataStore.GetTickInfo(SelectedTick?.ID);

            var systems = tickData.Item1.OrderBy(x => x.Name).Select(x => new BGSTickSystemVM(x));

            this.systems.ClearCollection();
            this.systems.AddRange(systems);

            var address = SelectedSystem == null ? dataStore.CurrentSystem?.Address ?? 0 : SelectedSystem.Address;

            SelectedSystem = system == null ? this.systems.FirstOrDefault(x => x.Address == address) : this.systems.FirstOrDefault(x => x.Address == system.Address);
            foreach (var mission in tickData.Item2)
            {
                switch (mission.CurrentState)
                {
                    case MissionState.Completed:
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
                    case MissionState.Failed:
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
        }

        private void PopulateMissions()
        {
            var missions = dataStore.Missions.Where(x => x.CurrentState < MissionState.Completed).Select(x => new BGSMissionVM(x));

            Missions.ClearCollection();
            Missions.AddRange(missions);
        }
        private void PopulateMegaships()
        {
            MegaShipScans = dataStore.MegaShipScans.Select(x => new MegaShipScanVM(x));
            OnPropertyChanged(nameof(MegaShipScans));
        }
    }
}
