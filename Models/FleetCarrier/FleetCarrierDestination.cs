namespace ODEliteTracker.Models.FleetCarrier
{
    public sealed class FleetCarrierDestination
    {
        public FleetCarrierDestination()
        {
            SystemName = "No Jump Set";
            BodyName = string.Empty;
        }

        public FleetCarrierDestination(string systemName, string bodyName, ulong systemAddress, DateTime departureTime)
        {
            SystemName = systemName;
            BodyName = bodyName;
            SystemAddress = systemAddress;
            DepartureTime = departureTime;
        }

        public string SystemName { get; set; }
        public string BodyName { get; set; }
        public ulong SystemAddress { get; set; }
        public DateTime DepartureTime { get; set; } = DateTime.MinValue;
        public bool Arrived => DepartureTime <= DateTime.UtcNow;

        public void Reset()
        {
            SystemName = "No Jump Set";
            BodyName = string.Empty;
        }
    }
}
