using ODEliteTracker.Models.Missions;
using ODMVVM.Extensions;
using ODMVVM.ViewModels;

namespace ODEliteTracker.ViewModels.ModelViews.BGS
{
    public sealed class BGSMissionVM(BGSMission mission) : ODObservableObject
    {
        private readonly BGSMission mission = mission;

        public string Origin => $"{mission.OriginSystemName} : {mission.OriginStationName}";
        public string OriginSystemName => mission.OriginSystemName;
        public string OriginStationName => mission.OriginStationName;
        public MissionState CurrentState => mission.CurrentState;
        public ulong MissionID => mission.MissionID;
        public string LocalisedName => mission.LocalisedName;
        public string IssuingFaction => mission.IssuingFaction;
        public string TargetFaction => mission.TargetFaction;
        public string Expiry => mission.Expiry.RelativeTime(DateTime.UtcNow);
        public string Destination
        {
            get
            {
                var stationName = string.IsNullOrEmpty(mission.DestinationSettlement) ? mission.DestinationStation : mission.DestinationSettlement;

                if (string.IsNullOrEmpty(stationName))
                    return mission.DestinationSystem;

                return $"{mission.DestinationSystem} : {stationName}";
            }
        }
        public string DestinationSystem => mission.DestinationSystem;
        public string? DestinationStation => mission.DestinationStation;
        public string? DestinationSettlement => mission.DestinationSettlement;

        public void UpdateExpiry()
        {
            OnPropertyChanged(nameof(Expiry));
        }

        public void UpdateDestination()
        {
            OnPropertyChanged(nameof(DestinationSystem));
            OnPropertyChanged(nameof(DestinationStation));
            OnPropertyChanged(nameof(DestinationSettlement));
        }
    }
}
