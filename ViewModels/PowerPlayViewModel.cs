using ODEliteTracker.Models.PowerPlay;
using ODEliteTracker.Stores;
using ODEliteTracker.ViewModels.ModelViews.PowerPlay;
using ODMVVM.Commands;
using ODMVVM.Helpers;
using ODMVVM.ViewModels;
using System.Windows.Input;

namespace ODEliteTracker.ViewModels
{
    public sealed class PowerPlayViewModel : ODViewModel
    {
        public PowerPlayViewModel(PowerPlayDataStore dataStore) 
        {
            this.dataStore = dataStore;
            this.dataStore.OnStoreLive += OnStoreLive;
            this.dataStore.PledgeDataUpdated += OnPledgeDataUpdated;
            this.dataStore.CyclesUpdated += OnCyclesUpdated;
            SetSelectedSystemCommand = new ODRelayCommand<PowerPlaySystemVM>(OnSetSelectedSystem);

            currentCycleNo = EliteHelpers.CurrentCycleNo();

            if (this.dataStore.IsLive)
                OnStoreLive(null, true);
        }

        private readonly PowerPlayDataStore dataStore;
        private int currentCycleNo;

        private PledgeDataVM? pledgeData;
        public PledgeDataVM? PledgeData
        {
            get => pledgeData;
            set
            {
                pledgeData = value;
                OnPropertyChanged(nameof(PledgeData));
            }
        }

        private int tabIndex;
        public int TabIndex
        {
            get => tabIndex;
            set
            {
                tabIndex = value;
                SetSelectedSystem();
                OnPropertyChanged(nameof(TabIndex));
                OnPropertyChanged(nameof(CurrentCycle));
                OnPropertyChanged(nameof(SelectedSystemData));
            }
        }
        public override bool IsLive => dataStore.IsLive;

        public ICommand SetSelectedSystemCommand { get; }
        public List<PowerPlaySystemVM>? LastCycleSystems { get; private set; }
        public List<PowerPlaySystemVM>? ThisCycleSystems { get; private set; }

        private PowerPlaySystemVM? selectedSystem;
        public PowerPlaySystemVM? SelectedSystem
        {
            get => selectedSystem;
            set
            {
                if(selectedSystem != null)
                    selectedSystem.IsSelected = false;
                selectedSystem = value;
                if (selectedSystem != null)
                    selectedSystem.IsSelected = true;
                OnPropertyChanged(nameof(SelectedSystem));
                OnPropertyChanged(nameof(SelectedSystemData));
            }
        }

        private PowerPlayCycleDataVM? selectedSystemData;
        public PowerPlayCycleDataVM? SelectedSystemData
        {
            get
            {
                if (selectedSystem is null)
                    return null;

                if (selectedSystem.Data.TryGetValue(GetSelectedCycle(), out var data))
                    return data;
                return null;
            }
        }

        private DateTime GetSelectedCycle()
        {
            if (tabIndex == 0)
                return dataStore.CurrentCycle;
            return dataStore.PreviousCycle;
        }

        public string CurrentCycle => $"Cycle 2.{currentCycleNo - tabIndex}";

        private void OnSetSelectedSystem(PowerPlaySystemVM model)
        {
            SelectedSystem = model;
        }

        private void OnCyclesUpdated(object? sender, EventArgs e)
        {
            currentCycleNo = EliteHelpers.CurrentCycleNo();
            UpdateSystems(null);
            OnPropertyChanged(nameof(CurrentCycle));
        }

        private void OnStoreLive(object? sender, bool e)
        {
            if(e)
            {
                UpdateSystems(null);

                if (dataStore.PledgeData != null)
                {
                    OnPledgeDataUpdated(null, dataStore.PledgeData);
                }
            }

            OnModelLive(e);
        }

        private void UpdateSystems(PowerPlaySystem? system)
        {
            LastCycleSystems = dataStore.Systems.Where(x => x.CycleData.ContainsKey(dataStore.PreviousCycle)/* && x.MeritsEarned > 0*/)
                                                .OrderBy(x => x.Name)
                                                .Select(x => new PowerPlaySystemVM(x)).ToList();

            ThisCycleSystems = dataStore.Systems.Where(x => x.CycleData.ContainsKey(dataStore.CurrentCycle)/* && x.MeritsEarned > 0*/)
                                                .OrderBy(x => x.Name)
                                                .Select(x => new PowerPlaySystemVM(x)).ToList();

            OnPropertyChanged(nameof(LastCycleSystems));
            OnPropertyChanged(nameof(ThisCycleSystems));

            if(system is not null)
            {
                SelectedSystem = ThisCycleSystems.FirstOrDefault(x => x.Address == system.Address);
                tabIndex = 0;
            }
            if (SelectedSystem is null)
            {
                SelectedSystem = ThisCycleSystems.FirstOrDefault();
                tabIndex = 0;
            }
            if (SelectedSystem is null)
            {
                SelectedSystem = LastCycleSystems.FirstOrDefault();
                tabIndex = 1;
            }

            OnPropertyChanged(nameof(TabIndex));
        }

        
        private void OnPledgeDataUpdated(object? sender, PledgeData? e)
        {
            if (e != null)
            {
                PledgeData = new(e);
                return;
            }
            PledgeData = null;
        }

        private void SetSelectedSystem()
        {
            if (SelectedSystem is null)
            {
                SelectedSystem = TabIndex == 0 ? ThisCycleSystems?.FirstOrDefault() : LastCycleSystems?.FirstOrDefault();
                return;
            }

            if(TabIndex == 0)
            {
                SelectedSystem = ThisCycleSystems?.FirstOrDefault(x => x.Address == SelectedSystem.Address) ?? ThisCycleSystems?.FirstOrDefault();
                return;
            }

            SelectedSystem = LastCycleSystems?.FirstOrDefault(x => x.Address == SelectedSystem.Address) ?? LastCycleSystems?.FirstOrDefault();
        }
        public override void Dispose()
        {
            this.dataStore.OnStoreLive -= OnStoreLive;
            this.dataStore.PledgeDataUpdated -= OnPledgeDataUpdated;
        }
    }
}
