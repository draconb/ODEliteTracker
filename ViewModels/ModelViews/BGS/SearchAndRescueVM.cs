using ODEliteTracker.Models.BGS;
using ODMVVM.ViewModels;

namespace ODEliteTracker.ViewModels.ModelViews.BGS
{
    public sealed class SearchAndRescueVM : ODObservableObject
    {
        public List<SearchAndRescueItem> Items { get; private set; } = [];

        public int Total => Items.Sum(x => x.Count);
        public string TotalCount => Total == 0 ? string.Empty : $"{Total:N0}";

        public void AddItem(SearchAndRescue item)
        {
            var known = Items.FirstOrDefault(x => x.ItemName == item.ItemName);

            if (known != null)
            {
                known.Update(item);
                return;
            }

            Items.Add(new(item));
        }
    }
}
