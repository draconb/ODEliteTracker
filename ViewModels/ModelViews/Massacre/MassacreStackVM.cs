using ODEliteTracker.Models.Missions;
using ODMVVM.Extensions;
using ODMVVM.ViewModels;
using System.Collections.ObjectModel;

namespace ODEliteTracker.ViewModels.ModelViews.Massacre
{
    public sealed class MassacreStackVM(string issuingFaction, string targetFaction, string starSystem) : ODObservableObject
    {
        public string IssuingFaction { get; } = issuingFaction;
        public string TargetFaction { get; } = targetFaction;
        public string? StarSystem { get; } = starSystem;
        public int Reward => ActiveMissions.Sum(x => x.Reward);
        public int Kills => ActiveMissions.Sum(x => x.KillCount);
        public int KillsRemaining => Missions.Sum(x => x.KillCount - x.Kills);

        private int killDifference;
        public int KillDifference
        {
            get => killDifference;
            set
            {
                killDifference = value;
                OnPropertyChanged(nameof(KillDifference));
            }
        }

        public string RewardString => $"{Reward:N0}";
        public int ActiveMissionCount => Missions.Count(x => x.CurrentState < MissionState.Completed);
        public ObservableCollection<MassacreMissionVM> Missions { get; } = [];
        public IEnumerable<MassacreMissionVM> ActiveMissions => Missions.Where(x => x.CurrentState < MissionState.Completed);
        public MassacreMissionVM? AddMission(MassacreMission mission)
        {
            var newMission = Missions.FirstOrDefault(x => x.MissionID == mission.MissionID);

            if (newMission != null)
                return null;

            newMission = new(mission);
            Missions.AddItem(newMission);
            OnPropertyChanged(nameof(Reward));
            OnPropertyChanged(nameof(RewardString));
            OnPropertyChanged(nameof(Kills));
            OnPropertyChanged(nameof(KillsRemaining));
            OnPropertyChanged(nameof(StarSystem));
            OnPropertyChanged(nameof(ActiveMissions));

            return newMission;
        }

        public bool UpdateMission(MassacreMission mission)
        {
            //Nothing to do if the mission is still active
            if (mission.CurrentState < MissionState.Completed)
                return false;

            var known = Missions.FirstOrDefault(x => x.MissionID == mission.MissionID);

            if (known == null)
                return false;

            OnPropertyChanged(nameof(ActiveMissionCount));
            OnPropertyChanged(nameof(RewardString));
            OnPropertyChanged(nameof(Kills));
            OnPropertyChanged(nameof(ActiveMissions));
            OnPropertyChanged(nameof(KillsRemaining));
            var ret = !ActiveMissions.Any();
            return ret;
        }

        internal void UpdateKills()
        {
            OnPropertyChanged(nameof(KillsRemaining));
        }
    }
}
