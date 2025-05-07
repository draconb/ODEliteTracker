using ODEliteTracker.Managers;
using ODMVVM.ViewModels;

namespace ODEliteTracker.ViewModels.ModelViews.Shared
{
    public sealed class BountyVM(BountyClaims claims) : ODObservableObject
    {
        private readonly BountyClaims claims = claims;

        public string Name { get; } = claims.FactionName;

        public long ValueLong = claims.Value;
        public string Value { get; } = $"{claims.Value:N0}";
        public int CountInt => claims.Count;
        public string Count { get; } = $"{claims.Count:N0}";
    }
}
