using ODEliteTracker.Models.BGS;
using ODEliteTracker.Models.Galaxy;
using ODMVVM.ViewModels;

namespace ODEliteTracker.ViewModels.ModelViews.BGS
{
    public sealed class BGSTickSystemVM : ODObservableObject
    {
        public BGSTickSystemVM(BGSTickSystem system) 
        { 
            this.system = system;
            Factions = [.. system.Factions.OrderByDescending(x => x.Influence).Select(x => new FactionVM(x))];

            foreach (var voucher in system.VoucherClaims)
            {
                switch (voucher.VoucherType)
                {
                    case Models.VoucherType.Bounty:
                        var faction = Factions.FirstOrDefault(x => string.Equals(x.Name,voucher.Faction,StringComparison.OrdinalIgnoreCase));

                        if (faction != null)
                            faction.Bounties += voucher.Value;
                        continue;
                    case Models.VoucherType.CombatBond:
                        var fction = Factions.FirstOrDefault(x => string.Equals(x.Name, voucher.Faction, StringComparison.OrdinalIgnoreCase));

                        if (fction != null)
                            fction.Bonds += voucher.Value;
                        continue;
                    default:
                        continue;
                }
            }

        }
        private readonly BGSTickSystem system;

        public string Name => system.Name.ToUpper();
        public long Address => system.Address;
        public Position Position => system.Position;
        public string ControllingFaction => system.ControllingFaction;
        public string SystemAllegiance => system.SystemAllegiance;
        public List<FactionVM> Factions { get; }

        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }
    }
}
