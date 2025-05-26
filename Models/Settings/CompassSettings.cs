namespace ODEliteTracker.Models.Settings
{
    [Flags]
    public enum CompassBools
    {
        None = 0,
        HideWhenInSRV = 1 << 1,
        HideWhenOnFoot = 1 << 2,
        HideWhenNoLongLat = 1 << 3,
        HideTargetWhenNotActive = 1 << 4,
    }

    public sealed class CompassSettings
    {
        public CompassBools Bools { get; set; } = CompassBools.HideWhenNoLongLat | CompassBools.HideTargetWhenNotActive;
        public double SpeedInShip { get; set; } = 0.03d;
        public double SpeedOnFoot { get; set; } = 0.05d;
    }
}
