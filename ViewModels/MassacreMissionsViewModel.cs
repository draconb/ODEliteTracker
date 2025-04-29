using ODEliteTracker.Models.Missions;
using ODEliteTracker.Stores;
using ODEliteTracker.ViewModels.ModelViews.Massacre;
using ODMVVM.Extensions;
using ODMVVM.ViewModels;
using System.Collections.ObjectModel;

namespace ODEliteTracker.ViewModels
{
    public sealed class MassacreMissionsViewModel : ODViewModel
    {
        public MassacreMissionsViewModel(MassacreMissionStore massacreStore)
        {
            this.massacreStore = massacreStore;
            this.massacreStore.StoreLive += OnStoreLive;
            this.massacreStore.MissionAddedEvent += OnMissionAdded;
            this.massacreStore.MissionUpdatedEvent += OnMissionUpdated;
            this.massacreStore.MissionsUpdatedEvent += OnMissionsUpdated;

            //Timer to update expiry time every minute
            expiryTimeUpdateTimer = new Timer(OnUpdateTimes, null, 0, 60000);

            if (this.massacreStore.IsLive)
            {
                OnStoreLive(null, true);
            }
        }

        private readonly MassacreMissionStore massacreStore;
        private readonly Timer expiryTimeUpdateTimer;

        private List<MassacreStackVM> stacks { get; } = [];
        public IEnumerable<MassacreStackVM> Stacks => stacks.Where(x => x.ActiveMissionCount > 0).OrderBy(x => x.IssuingFaction);
        public IEnumerable<MassacreMissionVM> ActiveMissions { get; private set; } = [];
        public IEnumerable<MassacreMissionVM> CompletedMissions { get; private set; } = [];
        public ObservableCollection<FactionStackVM> FactionStacks { get; private set; } = [];

        public override bool IsLive { get => massacreStore.IsLive; }
        public override void Dispose()
        {
            massacreStore.StoreLive -= OnStoreLive;
            massacreStore.MissionAddedEvent -= OnMissionAdded;
            massacreStore.MissionUpdatedEvent -= OnMissionUpdated;
            expiryTimeUpdateTimer.Dispose();
        }

        private void OnStoreLive(object? sender, bool e)
        {
            if (e)
            {
                BuildStacks();
                OnModelLive(true);
            }
        }

        private void BuildStacks()
        {
            foreach (var mission in massacreStore.Missions)
            {
                AddMissionToStack(mission);
            }

            var factionMissions = stacks.Where(x => x.ActiveMissionCount > 0)
                                        .SelectMany(x => x.Missions)
                                        .Where(x => x.CurrentState <= MissionState.Completed)
                                        .GroupBy(x => x.TargetFaction)
                                        .ToDictionary(x => x.Key, x => x.ToList());
            FactionStacks = [.. factionMissions.Select(x => new FactionStackVM(x.Key, x.Value))];

            SetMissionCollections();
            OnPropertyChanged(nameof(FactionStacks));
            OnPropertyChanged(nameof(Stacks));
        }

        private void SetMissionCollections()
        {
            ActiveMissions = stacks.Where(x => x.ActiveMissionCount > 0)
                                   .SelectMany(x => x.Missions)
                                   .Where(x => x.CurrentState < MissionState.Completed)
                                   .OrderBy(x => x.IssuingFaction)
                                   .ThenBy(m => m.AcceptedTime);

            CompletedMissions = stacks.SelectMany(x => x.Missions)
                                      .Where(x => x.CurrentState == MissionState.Completed)
                                      .OrderByDescending(x => x.CompletionTime);

            OnPropertyChanged(nameof(ActiveMissions));
            OnPropertyChanged(nameof(CompletedMissions));
        }
        private void OnMissionAdded(object? sender, MassacreMission e)
        {
            var mission = AddMissionToStack(e);
            SetMissionCollections();
            OnPropertyChanged(nameof(FactionStacks));
            OnPropertyChanged(nameof(Stacks));

            if (mission is null)
                return;

            var knownFactionStack = FactionStacks.FirstOrDefault(x => string.Equals(x.TargetFaction, e.TargetFaction));

            if(knownFactionStack != null)
            {
                knownFactionStack.AddMission(mission);
                return;
            }

            FactionStacks.AddItem(new(e.TargetFaction, [mission]));
           
        }

        private void OnMissionUpdated(object? sender, MassacreMission e)
        {
            var mission = stacks.SelectMany(x => x.Missions).FirstOrDefault(x => x.MissionID == e.MissionID);

            if (mission == null)
                return;

            mission.Update(e);

            if(e.CurrentState >= MissionState.Completed)
            {
                var stack = stacks.FirstOrDefault(x => string.Equals(x.IssuingFaction, mission.IssuingFaction));

                if (stack != null && stack.UpdateMission(e))
                {
                    stacks.Remove(stack);
                }
                SetMissionCollections();
                OnPropertyChanged(nameof(Stacks));
            }
            var factionStack = FactionStacks.FirstOrDefault(x => string.Equals(x.TargetFaction, e.TargetFaction));

            if (factionStack == null)
            {
                return;
            }

            if(factionStack.Update(mission) && factionStack.CurrentMissionCount == 0)
            {
                FactionStacks.RemoveItem(factionStack);
            }
        }

        private void OnMissionsUpdated(object? sender, EventArgs e)
        {
            stacks.Clear();
            FactionStacks.ClearCollection();
            BuildStacks();
        }

        private MassacreMissionVM? AddMissionToStack(MassacreMission mission)
        {
            var stack = stacks.FirstOrDefault(x => string.Equals(x.IssuingFaction, mission.IssuingFaction)
                                                && string.Equals(x.TargetFaction, mission.TargetFaction));

            if (stack is null)
            {
                stack = new MassacreStackVM(mission.IssuingFaction, mission.TargetFaction);
                stacks.Add(stack);
            }
            return stack.AddMission(mission);
        }

        private void OnUpdateTimes(object? state)
        {
            if (ActiveMissions.Any() == false)
                return;

            foreach (var mission in ActiveMissions)
            {
                mission.UpdateExpiryTime();
            }
        }
    }
}
