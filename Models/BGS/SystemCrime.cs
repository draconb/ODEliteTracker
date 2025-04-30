using EliteJournalReader;
using EliteJournalReader.Events;
using ODEliteTracker.Models.Galaxy;

namespace ODEliteTracker.Models.BGS
{
    public sealed class SystemCrime
    {
        public SystemCrime(CommitCrimeEvent.CommitCrimeEventArgs commitCrime, FactionData value)
        {
            EventTime = commitCrime.Timestamp;
            TargetFaction = value;

            switch (commitCrime.CrimeType)
            {
                case "murder":
                    ShipMurders++;
                    break;
                case "onFoot_murder":
                    OnFootMurders++;
                    break;
            }
        }

        public DateTime EventTime { get; }
        public int OnFootMurders { get; }
        public int ShipMurders { get; }
        public FactionData TargetFaction { get; }
    }
}
