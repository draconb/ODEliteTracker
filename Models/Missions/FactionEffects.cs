using ODMVVM.Helpers;
using static EliteJournalReader.Events.MissionCompletedEvent.MissionCompletedEventArgs;

namespace ODEliteTracker.Models.Missions
{
    public sealed class FactionEffects
    {
        public FactionEffects(FactionEffectsDesc factionEffects)
        {
            FactionName = factionEffects.Faction;
            Influence = [];
            foreach(var inf in factionEffects.Influence)
            {
                var positive = inf.Trend.StartsWith("Up", StringComparison.OrdinalIgnoreCase);
                var influence = inf.Influence.MissionPlusCount() * (positive ? 1 : -1);
                Influence.Add(new(inf.SystemAddress, influence));
            }
        }

        public string FactionName { get; }
        public List<MissionInfluence> Influence { get; }
    }
}
