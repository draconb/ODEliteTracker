using ODEliteTracker.Models.PowerPlay;
using ODMVVM.ViewModels;

namespace ODEliteTracker.ViewModels.ModelViews.PowerPlay
{
    public sealed class PledgeDataVM(PledgeData data) : ODObservableObject
    {
        public string Power => data.Power;
        public string Rank { get; private set; } = $"{data.Rank:N0}";
        public string Merits { get; private set; } = $"{data.Merits:N0}";
        public string MeritsEarnThisCycle { get; private set; } = $"{data.MeritsEarnedThisCycle:N0}";
        private TimeSpan timePledged => data.TimePledged + (DateTime.UtcNow - data.TimePledgedRecorded);
        public string TimePledged => $"{timePledged.Days}D {timePledged.Hours}H";

        public void UpdateMerits(long totalMerits)
        {
            Merits = $"{totalMerits:N0}";
            OnPropertyChanged(nameof(Merits));
        }
    }
}
