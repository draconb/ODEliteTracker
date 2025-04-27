using ODEliteTracker.Models.PowerPlay;
using ODMVVM.ViewModels;

namespace ODEliteTracker.ViewModels.ModelViews.PowerPlay
{
    public sealed class PledgeDataVM(PledgeData data) : ODObservableObject
    {
        public string Power => data.Power;
        public string Rank => $"{data.Rank:N0}";
        public string Merits => $"{data.Merits:N0}";
        public string TimePledged => $"{data.TimePledged.Days}D {data.TimePledged.Hours}H";
    }
}
