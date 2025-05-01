using EliteJournalReader;

namespace ODEliteTracker.Models.BGS
{
    public sealed class SystemConflict(Conflict conflict, DateTime eventTime)
    {
        public Conflict Conflict { get; } = conflict.Clone();

        public int Hash = conflict.Faction1.Name.GetHashCode()
            + conflict.Faction1.Stake.GetHashCode()
            + conflict.Faction2.Name.GetHashCode()
            + conflict.Faction2.Stake.GetHashCode();
        public List<DateTime> EventTimes { get; } = [eventTime];
    }
}
