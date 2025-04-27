using EliteJournalReader.Events;

namespace ODEliteTracker.Models.Missions
{
    public sealed class MassacreMission : MissionBase
    {
        public MassacreMission(MissionAcceptedEvent.MissionAcceptedEventArgs args,
                               long originAddress,
                               string originSystemName,
                               long originMarketID,
                               string originStationName,
                               bool odyssey) : base(args,
                                                                originAddress,
                                                                originSystemName,
                                                                originMarketID,
                                                                originStationName,
                                                                odyssey)
        {
            Target = args.Target;
            TargetType = args.TargetType;
            TargetFaction = args.TargetFaction;
            KillCount = args.KillCount ?? 0;
        }

        public string Target { get; set; }
        public string TargetType { get; set; }
        public string TargetFaction { get; set; }
        public int KillCount { get; set; }
        public int Kills { get; set; }  
    }
}
