using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ODEliteTracker.Database.DTOs
{
    [PrimaryKey(nameof(CommanderID), nameof(FactionName))]
    public class IgnoredBounties(int commanderID, string factionName, DateTime beforeDate)
    {
        public int CommanderID { get; set; } = commanderID;
        public string FactionName { get; set; } = factionName;
        public DateTime BeforeDate { get; set; } = beforeDate;
    }
}
