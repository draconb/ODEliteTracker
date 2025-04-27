namespace ODEliteTracker.ViewModels.ModelViews.PowerPlay
{
    public sealed class PowerPlayConflictDataVM
    {
        public PowerPlayConflictVM? WinningPower { get; set; }
        public List<PowerPlayConflictVM>? LosingPowers { get; set; }
        public string? ConflictState { get; set; }
    }
}
