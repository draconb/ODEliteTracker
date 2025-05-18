using EliteJournalReader.Events;

namespace ODEliteTracker.Models.PowerPlay
{
    public sealed class PledgeData(PowerplayEvent.PowerplayEventArgs args)
    {
        public string Power { get; set; } = args.Power;
        public int Rank { get; set;  } = args.Rank;
        public ulong Merits { get; set; } = args.Merits;
        public long MeritsEarnedThisCycle { get; set; } 
        public DateTime TimePledgedRecorded { get; set; } = args.Timestamp;
        public TimeSpan TimePledged { get; set; } = TimeSpan.FromSeconds(args.TimePledged);

        public void Update(PowerplayEvent.PowerplayEventArgs args)
        {
            if (string.Equals(Power, args.Power) == false)
            {
                MeritsEarnedThisCycle = 0;
                Power = args.Power;
            }

            Rank = args.Rank;
            Merits = args.Merits;
            TimePledgedRecorded = args.Timestamp;
        }
    }
}
