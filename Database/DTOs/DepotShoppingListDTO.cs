using Microsoft.EntityFrameworkCore;

namespace ODEliteTracker.Database.DTOs
{
    [PrimaryKey(nameof(MarketID), nameof(SystemAddress), nameof(StationName))]
    public record DepotShoppingListDTO(long MarketID, long SystemAddress, string StationName);
}
