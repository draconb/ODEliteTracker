namespace ODEliteTracker.Models.Galaxy
{
    public sealed class FactionData(string name, string government, string allegiance)
    {
        public string Name { get; set; } = name;
        public string Government { get; set; } = government;
        public string Allegiance { get; set; } = allegiance;
    }
}
