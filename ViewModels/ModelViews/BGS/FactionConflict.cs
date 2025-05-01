using EliteJournalReader;
using ODEliteTracker.Models;

namespace ODEliteTracker.ViewModels.ModelViews.BGS
{
    public sealed class FactionConflict(ConflictFaction faction, FactionConflictStatus status)
    {
        public string Name { get; } = faction.Name;
        public string Stake { get; } = string.IsNullOrEmpty(faction.Stake) ? "No assets at risk" : faction.Stake;
        public FactionConflictStatus Status { get; } = status;
    }
}
