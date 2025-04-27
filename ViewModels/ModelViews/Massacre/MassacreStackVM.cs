using ODEliteTracker.Models.Missions;
using ODMVVM.Extensions;
using ODMVVM.ViewModels;
using System.Collections.ObjectModel;

namespace ODEliteTracker.ViewModels.ModelViews.Massacre
{
    public sealed class MassacreStackVM(string issuingFaction, string targetFaction) : ODObservableObject
    {
        public string IssuingFaction { get; } = issuingFaction;
        public string TargetFaction { get; } = targetFaction;
        public int Reward => Missions.Sum(x => x.Reward);
        public int Kills => Missions.Sum(x => x.KillCount);
        public string RewardString => $"{Reward:N0}";
        public int ActiveMissionCount => Missions.Count(x => x.CurrentState < MissionState.Completed);
        public ObservableCollection<MassacreMissionVM> Missions { get; } = [];

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
            OnPropertyChanged(nameof(Kills));

            return Missions.Count == 0;
        }
    }
}
