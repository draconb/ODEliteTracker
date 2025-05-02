using System.Security.Policy;

namespace ODEliteTracker.ViewModels.ModelViews.PowerPlay
{
    public sealed class PPMeritsVM
    {
        public PPMeritsVM(string activity, int merits, int count)
        {
            Activity = activity;
            MeritsValue = merits;
            CountValue = count;
        }

        public string Activity { get; }
        public int MeritsValue { get; }
        public string Merits => $"{MeritsValue:N0}";
        public int CountValue { get; }
        public string Count => $"{CountValue:N0}";
    }
}
