using ODEliteTracker.Stores;
using ODEliteTracker.ViewModels.ModelViews.BGS;
using ODMVVM.Commands;
using ODMVVM.Extensions;
using ODMVVM.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ODEliteTracker.ViewModels
{
    public sealed class BGSViewModel : ODViewModel
    {
        public BGSViewModel(BGSDataStore dataStore)
        {
            this.dataStore = dataStore;
            this.dataStore.StoreLive += OnStoreLive;

            SetSelectedSystemCommand = new ODRelayCommand<BGSTickSystemVM>(OnSetSelectedSystem);
            if (dataStore.IsLive)
                OnStoreLive(null, true);
        }

        private void OnSetSelectedSystem(BGSTickSystemVM vM)
        {
            SelectedSystem = vM;
        }

        private readonly BGSDataStore dataStore;
        public override bool IsLive => dataStore.IsLive;

        public ICommand SetSelectedSystemCommand { get; }

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
        public ObservableCollection<BGSTickSystemVM> Systems { get; set; } = [];

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

                var ticks = dataStore.TickData.Select(x => new TickDataVM(x));
                Ticks.AddRange(ticks);

                SelectedTick = Ticks.FirstOrDefault(x => string.Equals(x.ID, dataStore.SelectedTick?.ID)) ?? Ticks.LastOrDefault();
                PopulateSystems();

                OnPropertyChanged(nameof(Ticks));
                OnModelLive(true);
            }
        }

        private void PopulateSystems()
        {
            var tickData = dataStore.GetTickInfo(SelectedTick?.ID);

            var systems = tickData.Item1.OrderBy(x => x.Name).Select(x => new BGSTickSystemVM(x));

            Systems.ClearCollection();
            Systems.AddRange(systems);
            SelectedSystem = Systems.FirstOrDefault();
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
        }
    }
}
