using EliteJournalReader.Events;

namespace ODEliteTracker.Models.Galaxy
{
    public sealed class Station
    {
        public Station(CarrierJumpEvent.CarrierJumpEventArgs e, FactionData data)
        {
            StationName = e.StationName;
            StationFaction = data;
            MarketID = e.MarketID;
            StationType = e.StationType;
        }

        public Station(LocationEvent.LocationEventArgs e, FactionData data)
        {
            StationName = e.StationName;
            StationFaction = data;
            MarketID = e.MarketID;
            StationType = e.StationType;
        }

        public Station(DockedEvent.DockedEventArgs e, FactionData data)
        {
            StationName = e.StationName;
            StationFaction = data;
            MarketID = e.MarketID;
            StationType = e.StationType;
        }

        public string StationName { get; set; }
        public FactionData StationFaction { get; set; }
        public ulong MarketID { get; set; }
        public string StationType { get; set; }
    }
}
