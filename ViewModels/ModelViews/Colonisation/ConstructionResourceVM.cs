using ODEliteTracker.Models.Colonisation;
using ODEliteTracker.Models.FleetCarrier;
using ODEliteTracker.ViewModels.ModelViews.FleetCarrier;
using ODMVVM.Helpers;
using ODMVVM.ViewModels;

namespace ODEliteTracker.ViewModels.ModelViews.Colonisation
{
    public sealed class ConstructionResourceVM : ODObservableObject
    {
        public ConstructionResourceVM(ConstructionResource resource)
        {
            FDEVName = resource.FDEVName;
            details = EliteCommodityHelpers.GetCommodityDetails(resource.FDEVName);
            LocalName = resource.LocalName ?? details.EnglishName;
            RequiredAmount = resource.RequiredAmount;
            ProvidedAmount = resource.ProvidedAmount;
            Payment = resource.Payment;
        }

        private Commodity details;
        public string FDEVName { get; set; }
        public string LocalName { get; set; }
        public string Category => details.EnglishCategory;
        public int RequiredAmount { get; set; }
        public int ProvidedAmount { get; set; }
        public int RemainingCount => RequiredAmount - ProvidedAmount;
        public string Required => $"{RequiredAmount:N0} t";
        public string Delivered => ProvidedAmount > 0 ? $"{ProvidedAmount:N0} t" : string.Empty;
        public string Remaining => $"{RemainingCount:N0} t";
        public int Payment { get; set; }
        public long CarrierStockValue { get; private set; }
        public string CarrierStock => CarrierStockValue > 0 ? $"{CarrierStockValue:N0} t" : string.Empty;

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

        internal void SetCarrierStock(CarrierCommodity carrierCommodity)
        {
            CarrierStockValue = carrierCommodity.StockCount;
            OnPropertyChanged(nameof(CarrierStock));
        }
    }
}
