using ODEliteTracker.Models.Ship;
using ODMVVM.ViewModels;

namespace ODEliteTracker.ViewModels.ModelViews
{
    public sealed class ShipCargoVM(ShipCargo item) : ODObservableObject
    {
        public string Name { get; set; } = item.Name;
        public int Count { get; set; } = item.Count;
    }
}
