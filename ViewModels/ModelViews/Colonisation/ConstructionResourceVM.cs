using ODEliteTracker.Models.Colonisation;
using ODEliteTracker.Models.FleetCarrier;
using ODMVVM.Helpers;
using ODMVVM.ViewModels;

namespace ODEliteTracker.ViewModels.ModelViews.Colonisation
{
    public sealed class ConstructionResourceVM : ODObservableObject
    {
        public ConstructionResourceVM(ConstructionResource resource)
        {
            FDEVName = resource.FDEVName;
            commodity = EliteCommodityHelpers.GetCommodityDetails(resource.FDEVName);
            LocalName = resource.LocalName ?? commodity.EnglishName;
            RequiredAmount = resource.RequiredAmount;
            ProvidedAmount = resource.ProvidedAmount;
            Payment = resource.Payment;
        }

        public ConstructionResourceVM(ConstructionResourceVM resource)
        {
            FDEVName = resource.FDEVName;
            commodity = resource.commodity;
            LocalName = resource.LocalName ?? commodity.EnglishName;
            RequiredAmount = resource.RequiredAmount;
            ProvidedAmount = resource.ProvidedAmount;
            Payment = resource.Payment;
        }

        public Commodity commodity { get; private set; }
        public string FDEVName { get; set; }
        public string LocalName { get; set; }
        public string Category => commodity.EnglishCategory;
        public int RequiredAmount { get; set; }
        public int ProvidedAmount { get; set; }
        public int RemainingCount => RequiredAmount - ProvidedAmount;
        public string Required => $"{RequiredAmount:N0} t";
        public string Delivered => ProvidedAmount > 0 ? $"{ProvidedAmount:N0} t" : string.Empty;
        public string Remaining => $"{RemainingCount:N0} t";
        public int Payment { get; set; }
        public long CarrierStockValue { get; set; }
        public string CarrierStock => CarrierStockValue > 0 ? $"{CarrierStockValue:N0} t" : string.Empty;
        public long MarketStockValue { get; private set; }
        public string MarketStock => MarketStockValue > 0 ? $"{MarketStockValue:N0} t" : string.Empty;
        public string CarrierStockDiff
        {
            get
            {
                if (RemainingCount <= 0 || CarrierStockValue <= 0)
                    return string.Empty;

                return $"{CarrierStockValue - RemainingCount:N0} t";
            }
        }

        public MarketPurchaseVM? FirstPurchase { get; set; }

        internal void Update(ConstructionResource resource)
        {
            LocalName = resource.LocalName ?? "Unknown";
            RequiredAmount = resource.RequiredAmount;
            ProvidedAmount = resource.ProvidedAmount;
            Payment = resource.Payment;

            OnPropertyChanged(nameof(LocalName));
            OnPropertyChanged(nameof(Required));
            OnPropertyChanged(nameof(Delivered));
            OnPropertyChanged(nameof(Remaining));
            OnPropertyChanged(nameof(Payment));
            OnPropertyChanged(nameof(RemainingCount));
        }

        internal void UpdateShoppingList()
        {
            OnPropertyChanged(nameof(Remaining));
        }

        internal void UpdateMarketStock(long value)
        {
            MarketStockValue = value;
            OnPropertyChanged(nameof(MarketStock));
        }

        internal void UpdatePurchase(MarketPurchaseVM marketPurchaseVM)
        {
            FirstPurchase = marketPurchaseVM;
            OnPropertyChanged(nameof(FirstPurchase));
        }

        internal void SetCarrierStock(long value)
        {
            CarrierStockValue = value;
            OnPropertyChanged(nameof(CarrierStock));
        }

        internal void UpdateCarrierStock(CarrierCommodity? item)
        {
            CarrierStockValue = item?.StockCount ?? 0;
            OnPropertyChanged(nameof(CarrierStock));
        }
    }
}
