using EliteJournalReader.Events;
using ODEliteTracker.Models.Galaxy;

namespace ODEliteTracker.Models.BGS
{
    public sealed class SearchAndRescue(SearchAndRescueEvent.SearchAndRescueEventArgs arg, FactionData data)
    {
        public DateTime EventTime { get; } = arg.Timestamp;
        public FactionData Faction {  get; } = data;
        public string ItemName { get; } = string.IsNullOrEmpty(arg.Name_Localised) ? arg.Name : arg.Name_Localised;
        public int Count { get; } = arg.Count;
        public int Reward { get; } = arg.Reward;
    }
}
