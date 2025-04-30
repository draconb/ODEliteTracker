
using EliteJournalReader;
using ODEliteTracker.Models.Galaxy;

namespace ODEliteTracker.Models.BGS
{
    public sealed class BGSTickSystem
    {
        public BGSTickSystem(BGSStarSystem starSystem, SystemTickData tickData, IEnumerable<VoucherClaim> claims, IEnumerable<TradeTransaction> transactions, IEnumerable<SystemCrime> crimes, IEnumerable<ExplorationData> carto, IEnumerable<SearchAndRescue> s_r, IEnumerable<SystemConflict> conflicts) 
        { 
            Name = starSystem.Name;
            Address = starSystem.Address;
            Population = tickData.Population;
            Position = starSystem.Position;
            ControllingFaction = tickData.ControllingFaction;
            SystemAllegiance = tickData.SystemAllegiance;
            VoucherClaims = [.. claims];
            Factions = tickData.Factions;
            Transactions = [.. transactions];
            Crimes = [.. crimes];
            Carto = [.. carto];
            SearchAndRescueData = [.. s_r];
            Conflicts = [.. conflicts];
            Security = starSystem.Security;
        }

        public string Name { get; }
        public long Address { get;  }
        public long Population { get; }
        public Position Position { get; }
        public string ControllingFaction { get;  }
        public string SystemAllegiance { get; }
        public string? Security { get; }
        public List<VoucherClaim> VoucherClaims { get; }
        public List<Faction> Factions { get; } = [];
        public List<TradeTransaction> Transactions { get;  } 
        public List<SystemCrime> Crimes { get;  }
        public List<ExplorationData> Carto { get; }
        public List<SearchAndRescue> SearchAndRescueData { get; }
        public List<SystemConflict> Conflicts { get; }

    }
}
