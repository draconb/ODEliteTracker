using static EliteJournalReader.Events.RedeemVoucherEvent.RedeemVoucherEventArgs;

namespace ODEliteTracker.Models.BGS
{
    public readonly struct VoucherClaim
    {
        public VoucherClaim(string type, FactionAmount factionAmount, DateTime timeClaimed)
        {
             Faction = factionAmount.Faction;
            Value = factionAmount.Amount;
            TimeClaimed = timeClaimed;

            if (Enum.TryParse(type, true, out VoucherType voucherType))
            {
                VoucherType = voucherType;
            }
        }

        public VoucherClaim(string type, string faction, long amount, DateTime timestamp)
        {
            Faction = faction;
            Value = amount;
            TimeClaimed = timestamp;

            if (Enum.TryParse(type, true, out VoucherType voucherType))
            {
                VoucherType = voucherType;
            }
        }

        public VoucherType VoucherType { get; } = VoucherType.Unknown;
        public string Faction { get; }
        public long Value { get; }
        public DateTime TimeClaimed { get; }
    }
}
