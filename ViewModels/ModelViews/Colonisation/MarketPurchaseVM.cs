using ODEliteTracker.Models.Market;
using ODMVVM.ViewModels;

namespace ODEliteTracker.ViewModels.ModelViews.Colonisation
{
    public sealed class MarketPurchaseVM(CommodityPurchase purchase, Models.Galaxy.StarSystem? currentSystem) : ODObservableObject
    {
        private readonly CommodityPurchase purchase = purchase;

        public string SystemName => purchase.Station.StarSystem.Name;
        public string StationName => purchase.Station.StationName;
        public string Price => $"{purchase.Price:N0} cr";
        public double Distance { get; private set; } = currentSystem == null ? 0 : currentSystem.Position.DistanceFrom(purchase.Station.StarSystem.Position);
        public string DistanceString => $"{Distance:N2} ly";

        public override string ToString()
        {
            return $"{SystemName} - {StationName}";
        }

        public void SetDistance(ConstructionDepotVM constructionDepotVM)
        {
            Distance = purchase.Station.StarSystem.Position.DistanceFrom(constructionDepotVM.starSystem.Position);
            OnPropertyChanged(nameof(DistanceString));
        }

        public void SetDistance(Models.Galaxy.StarSystem? currentSystem)
        {
            Distance = currentSystem == null ? 0 : purchase.Station.StarSystem.Position.DistanceFrom(currentSystem.Position);
            OnPropertyChanged(nameof(DistanceString));
        }
    }
}
