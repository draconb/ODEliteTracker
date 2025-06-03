namespace ODEliteTracker.Models.Settings
{
    [Flags]
    public enum ColonisationColumns
    {
        None = 0,
        Name = 1 << 0,
        Category = 1 << 1,
        MarketStock = 1 << 2,
        CarrierStock = 1 << 3,
        Remaining = 1 << 4,
        CarrierDiff = 1 << 5,
        All = ~None,
    }

    internal class ColonisationPopOutSettings
    {
        public ColonisationColumns Columns { get; set; } = ColonisationColumns.All;

        public bool ShowColumnHeaders { get; set; } = true;
    }
}
