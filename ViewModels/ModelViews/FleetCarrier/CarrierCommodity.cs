using ODEliteTracker.Models.FleetCarrier;

namespace ODEliteTracker.ViewModels.ModelViews.FleetCarrier
{
    public sealed class CarrierCommodityVM(CarrierCommodity commodity)
    {
        private readonly CarrierCommodity commodity = commodity;

        public string Name => commodity.EnglishName;
        public string Category => commodity.EnglishCategory;
        public string Stock => commodity.StockCount == 0 ? string.Empty : $"{commodity.StockCount:N0} t";
        public string Demand => commodity.BuyOrderCount == 0 ? string.Empty : $"{commodity.BuyOrderCount:N0} t";
    }
}
