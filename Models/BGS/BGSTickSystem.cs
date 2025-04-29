
using EliteJournalReader;
using ODEliteTracker.Models.Galaxy;

namespace ODEliteTracker.Models.BGS
{
    public sealed class BGSTickSystem
    {
        public BGSTickSystem(BGSStarSystem starSystem, SystemTickData tickData, IEnumerable<VoucherClaim> claims) 
        { 
            Name = starSystem.Name;
            Address = starSystem.Address;
            Position = starSystem.Position;
            ControllingFaction = tickData.ControllingFaction;
            SystemAllegiance = tickData.SystemAllegiance;
            VoucherClaims = [.. claims];
            Factions = tickData.Factions;
        }
        public string Name { get; }
        public long Address { get;  }
        public Position Position { get; }
        public string ControllingFaction { get;  }
        public string SystemAllegiance { get; }
        public List<VoucherClaim> VoucherClaims { get; } = [];
        public List<Faction> Factions { get; } = [];
    }
}
