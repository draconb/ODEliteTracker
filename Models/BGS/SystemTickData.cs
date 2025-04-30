using EliteJournalReader;
using EliteJournalReader.Events;

namespace ODEliteTracker.Models.BGS
{
    public sealed class SystemTickData
    {
        public SystemTickData(LocationEvent.LocationEventArgs evt)
        {
            VisitedTimes = [evt.Timestamp];
            ControllingFaction = evt.SystemFaction?.Name ?? "Unknown";
            SystemAllegiance = evt.SystemAllegiance;
            Population = evt.Population ?? 0;

            if (evt.Factions != null && evt.Factions.Count != 0)
            {
                Factions = [.. evt.Factions.Select(x => x.Clone())];
            }
        }

        public SystemTickData(FSDJumpEvent.FSDJumpEventArgs evt)
        {
            VisitedTimes = [evt.Timestamp];
            ControllingFaction = evt.SystemFaction?.Name ?? "Unknown";
            SystemAllegiance = evt.SystemAllegiance;
            Population = evt.Population;

            if (evt.Factions != null && evt.Factions.Count != 0)
            {
                Factions = [.. evt.Factions.Select(x => x.Clone())];
            }
        }

        public SystemTickData(CarrierJumpEvent.CarrierJumpEventArgs evt)
        {
            VisitedTimes = [evt.Timestamp];
            ControllingFaction = evt.SystemFaction?.Name ?? "Unknown";
            SystemAllegiance = evt.SystemAllegiance;
            Population = evt.Population ?? 0;

            if (evt.Factions != null && evt.Factions.Count != 0)
            {
                Factions = [.. evt.Factions.Select(x => x.Clone())];
            }
        }

        public List<DateTime> VisitedTimes { get; }
        public string ControllingFaction { get;  }
        public string SystemAllegiance { get;  }
        public long Population { get; }
        public List<Faction> Factions { get; } = [];

        public bool NewData(SystemTickData other)
        {
            var ret = !string.Equals(ControllingFaction, other.ControllingFaction);

            if (ret == true)
                return true;

            if(Factions.Count != other.Factions.Count) 
                return true;

            
            foreach (var faction in other.Factions)
            {
                var known = Factions.FirstOrDefault(x => x.Name == faction.Name);

                if (known == null)
                {
                    return true;
                }

                if(known.BGSDataUpdated(faction) == false)
                {
                    return true;
                }
            }

            //Data is unchanged so add another visit to this
            VisitedTimes.AddRange(other.VisitedTimes);
            return false;
        }

        public bool VisitedDuringPeriod(DateTime from, DateTime to)
        {
            var min = VisitedTimes.Min();
            var max = VisitedTimes.Max();

            return min >= from && max < to;
        }
    }
}
