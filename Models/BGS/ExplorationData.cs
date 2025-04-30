using EliteJournalReader.Events;
using ODEliteTracker.Models.Galaxy;

namespace ODEliteTracker.Models.BGS
{
    public sealed class ExplorationData
    {
        public ExplorationData(SellExplorationDataEvent.SellExplorationDataEventArgs args, FactionData data)
        {
            EventTime = args.Timestamp;
            Faction = data;
            Value = args.TotalEarnings;
        }

        public ExplorationData(MultiSellExplorationDataEvent.MultiSellExplorationDataEventArgs args, FactionData data)
        {
            EventTime = args.Timestamp;
            Faction = data;
            Value = args.TotalEarnings;
        }

        public DateTime EventTime { get; }
        public FactionData Faction { get; }
        public long Value { get; }
    }
}
