using ODEliteTracker.Models.BGS;

namespace ODEliteTracker.ViewModels.ModelViews.BGS
{
    public sealed class SystemConflictVM(SystemConflict conflict)
    {
        public string WarType
        {
            get
            {
                return conflict.Conflict.WarType switch
                {
                    "civilwar" => "Civil War",
                    "election" => "Election",
                    "war" => "War",
                    _ => string.Empty,
                };
            }
        }

        public string Status
        {
            get
            {
                return conflict.Conflict.Status switch
                {
                    "pending" => "Pending",
                    "active" => "Active",
                    _ => "Concluded",
                };
            }
        }

        public string Faction1Name { get; } = conflict.Conflict.Faction1.Name;
        public string Faction1Stake { get; } = string.IsNullOrEmpty(conflict.Conflict.Faction1.Stake) ? "No assets at risk" : conflict.Conflict.Faction1.Stake;

        public string Score { get; } = $"{conflict.Conflict.Faction1.WonDays} vs {conflict.Conflict.Faction2.WonDays}";

        public string Faction2Name { get; } = conflict.Conflict.Faction2.Name;
        public string Faction2Stake { get; } = string.IsNullOrEmpty(conflict.Conflict.Faction2.Stake) ? "No assets at risk" : conflict.Conflict.Faction2.Stake;
    }
}
