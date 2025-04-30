using ODEliteTracker.Models.BGS;
using ODMVVM.ViewModels;

namespace ODEliteTracker.ViewModels.ModelViews.BGS
{
    public sealed class SearchAndRescueItem(SearchAndRescue item) : ODObservableObject
    {
        public string ItemName { get; } = item.ItemName;
        public int Count { get; private set; } = item.Count;
        public int Reward { get; private set; } = item.Reward;

        public void Update(SearchAndRescue item)
        {
            Count += item.Count;
            Reward += item.Reward;
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(nameof(Reward));
        }
    }
}
