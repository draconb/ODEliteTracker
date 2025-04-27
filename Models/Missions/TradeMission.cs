using EliteJournalReader.Events;

namespace ODEliteTracker.Models.Missions
{
    public sealed class TradeMission(MissionAcceptedEvent.MissionAcceptedEventArgs args,
                        long originAddress,
                        string originSystemName,
                        long originMarketID,
                        string originStationName,
                        bool odyssey) : MissionBase(args, originAddress, originSystemName, originMarketID, originStationName, odyssey)
    {
        public string FdevCommodity { get; } = args.Commodity;
        public string Commodity_Localised { get; } = args.Commodity_Localised;
        public int Count { get; } = args.Count ?? 0;
        public int ItemsCollected { get; set; }
        public int ItemsDelivered { get; set; }
    }
}
