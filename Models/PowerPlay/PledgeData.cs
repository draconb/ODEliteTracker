using EliteJournalReader.Events;

namespace ODEliteTracker.Models.PowerPlay
{
    public sealed class PledgeData(PowerplayEvent.PowerplayEventArgs args)
    {
        public string Power { get; set; } = args.Power;
        public int Rank { get; set;  } = args.Rank;
        public ulong Merits { get; set; } = args.Merits;
        public TimeSpan TimePledged { get; set; } = TimeSpan.FromSeconds(args.TimePledged);
    }
}
