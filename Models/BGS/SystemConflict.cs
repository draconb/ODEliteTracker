using EliteJournalReader;

namespace ODEliteTracker.Models.BGS
{
    public sealed class SystemConflict(Conflict conflict, DateTime eventTime)
    {
        public Conflict Conflict { get; } = conflict.Clone();
        public List<DateTime> EventTimes { get; } = [eventTime];
    }
}
