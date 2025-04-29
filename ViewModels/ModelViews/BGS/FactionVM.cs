using EliteJournalReader;
using ODMVVM.Helpers;
using ODMVVM.ViewModels;

namespace ODEliteTracker.ViewModels.ModelViews.BGS
{
    public sealed class FactionVM(Faction faction) : ODObservableObject
    {
        public string Name { get; } = faction.Name;
        public string FactionState { get; } = faction.FactionState.SplitCamelCase();
        public string Government { get; } = string.IsNullOrEmpty(faction.Government_Localised) ? faction.Government : faction.Government_Localised;
        public string Influence { get; } = $"{faction.Influence * 100:N2} %";
        public string Allegiance { get; } = faction.Allegiance;
        public string MyReputation { get; } = EliteHelpers.FactionReputationToString(faction.MyReputation);
        public bool SquadronFaction { get; } = faction.SquadronFaction;

        private int infPlus;
        public int InfPlus
        {
            get => infPlus;
            set
            {
                infPlus = value;
                OnPropertyChanged(nameof(InfPluses));
            }
        }
        public string InfPluses => infPlus == 0 ? string.Empty : $"{InfPlus:N0}";

        private long bounties;
        public long Bounties
        {
            get => bounties;
            set
            {
                bounties = value;
                OnPropertyChanged(nameof(BountyVouchers));
            }
        }
        public string BountyVouchers => bounties == 0 ? string.Empty : EliteHelpers.FormatNumber(bounties);

        private long bonds;
        public long Bonds
        {
            get => bonds;
            set
            {
                bonds = value;
                OnPropertyChanged(nameof(BondVouchers));
            }
        }
        public string BondVouchers => bonds == 0 ? string.Empty : EliteHelpers.FormatNumber(bonds);

        private int failed;
        public int Failed
        {
            get => failed;
            set
            {
                failed = value;
                OnPropertyChanged(nameof(MissionsFailed));
            }
        }
        public string MissionsFailed => failed == 0 ? string.Empty : $"{failed:N0}";
    }
}
