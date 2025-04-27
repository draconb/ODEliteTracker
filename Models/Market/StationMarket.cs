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
        }

        public long MarketID { get; set; }
        public string StationName { get; set; }
        public string StarSystem { get; set; }
        public List<StationCommodity> ItemsForSale { get; set; }
    }
}
