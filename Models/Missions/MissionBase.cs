using EliteJournalReader.Events;
using ODMVVM.Helpers;

namespace ODEliteTracker.Models.Missions
{
    public enum MissionState
    {
        Active,
        Redirected,
        Completed,
        Failed,
        Abandoned
    }

    public class MissionBase
    {
        public MissionBase(MissionAcceptedEvent.MissionAcceptedEventArgs args,
                           long originAddress,
                           string originSystemName,
                           ulong originMarketID,
                           string originStationName,
                           bool odyssey) 
        { 
            Odyssey = odyssey;
            OriginSystemAddress = originAddress;
            OriginSystemName = originSystemName;
            OriginMarketID = originMarketID;
            OriginStationName = originStationName;
            MissionID = args.MissionID;
            Name = args.Name;
            LocalisedName = args.LocalisedName;
            IssuingFaction = args.Faction;
            Influence = args.Influence.MissionPlusCount();
            Reputation = args.Reputation.MissionPlusCount();
            Reward = args.Reward;
            Wing = args.Wing;
            Expiry = args.Expiry ?? DateTime.MinValue;
            CurrentState = MissionState.Active;
            AcceptedTime = args.Timestamp;
            DestinationSystem = args.DestinationSystem;
            DestinationSettlement = args.DestinationSettlement;
            DestinationSystem = args.DestinationSystem;
        }
        public bool Odyssey { get; private set; }
        public long OriginSystemAddress { get; private set; }
        public string OriginSystemName { get; private set; }
        public ulong OriginMarketID { get; private set; }
        public string OriginStationName { get; private set; }
        public MissionState CurrentState { get; set;}
        public DateTime AcceptedTime { get; private set;}
        public ulong MissionID { get; set; }
        public string Name { get; set; }
        public string LocalisedName { get; set; }
        public string IssuingFaction { get; set; }
        public int Influence { get; set; }
        public int Reputation { get; set; }
        public int Reward { get; set; }
        public bool Wing { get; set; }
        public DateTime Expiry { get; set; }
        public DateTime CompletionTime { get; set; }
        public string DestinationSystem { get; set; }
        public string? DestinationStation { get; set; }
        public string? DestinationSettlement { get; set; }

        public void UpdateState(MissionState state, DateTime updateTime)
        {
            CurrentState = state;
        }
    }
}
