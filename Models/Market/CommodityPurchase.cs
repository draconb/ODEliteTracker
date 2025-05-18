using ODEliteTracker.Models.Galaxy;
using ODMVVM.Helpers;

namespace ODEliteTracker.Models.Market
{
    public record CommodityPurchase(Commodity Commodity, Station Station, int Price, DateTime PurchaseDate);
}
