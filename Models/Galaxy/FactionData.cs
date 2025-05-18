namespace ODEliteTracker.Models.Galaxy
{
    public sealed class FactionData
    {
        public FactionData(string name, string government, string allegiance)
        {
            Name = name;
            Government = government;
            Allegiance = allegiance;
        }

        public FactionData(StarSystem currentSystem)
        {
            Name = currentSystem.ControllingFaction ?? string.Empty;
            Allegiance = currentSystem.SystemAllegiance ?? string.Empty;
            Government = string.Empty;
        }

        public string Name { get; }
        public string Government { get; }
        public string Allegiance { get; }
    }
}
