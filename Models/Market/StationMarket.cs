using EliteJournalReader.Events;

namespace ODEliteTracker.Models.Market
{
    public sealed class StationMarket
    {
        public StationMarket(MarketInfo marketInfo)
        {
            MarketID = marketInfo.MarketID;
            StationName = marketInfo.StationName;
            StarSystem = marketInfo.StarSystem;
            ItemsForSale = [.. marketInfo.Items.Where(x => x.Stock > 0).Select(x => new StationCommodity(x))];
            ItemsForPurchase = [.. marketInfo.Items.Where(x => x.Demand > 0).Select(x => new StationCommodity(x))];
        }

        public ulong MarketID { get; set; }
        public string StationName { get; set; }
        public string StarSystem { get; set; }
        public List<StationCommodity> ItemsForSale { get; set; }
        public List<StationCommodity> ItemsForPurchase { get; set; }
    }
}
