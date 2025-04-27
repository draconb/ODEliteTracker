using EliteJournalReader.Events;

namespace ODEliteTracker.Models.Missions
{
    public sealed class BGSMission(MissionAcceptedEvent.MissionAcceptedEventArgs args,
                                   long originAddress,
                                   string originSystemName,
                                   long originMarketID,
                                   string originStationName,
                                   bool odyssey) : MissionBase(args, 
                                                               originAddress,
                                                               originSystemName,
                                                               originMarketID,
                                                               originStationName,
                                                               odyssey)
    {
        public List<FactionEffects>? FactionEffects { get; private set; }

        internal void ApplyFactionEffects(IReadOnlyList<MissionCompletedEvent.MissionCompletedEventArgs.FactionEffectsDesc> factionEffects)
        {
            FactionEffects = [];

            foreach (var factionEffect in factionEffects)
            {
                if (string.IsNullOrEmpty(factionEffect.Faction))
                    continue;

                var effect = new FactionEffects(factionEffect);
                FactionEffects.Add(effect);
            }
        }
    }
}
