using ODEliteTracker.Database.DTOs;

namespace ODEliteTracker.ViewModels.ModelViews.BGS
{
    public sealed class TickDataVM(BGSTickData data)
    {
        public string ID { get; } = data.Id;
        public string Time { get; } = data.Time.AddYears(1286).ToString("dd MMM yyyy HH:mm");
    }
}
