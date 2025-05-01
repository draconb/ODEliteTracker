using ODEliteTracker.Models;
using ODEliteTracker.Models.BGS;

namespace ODEliteTracker.ViewModels.ModelViews.BGS
{
    public sealed class SystemConflictVM(SystemConflict data)
    {
        public string WarType
        {
            get
            {
                return data.Conflict.WarType switch
                {
                    "civilwar" => "Civil War",
                    "election" => "Election",
                    "war" => "War",
                    _ => string.Empty,
                };
            }
        }

        public ConflictStatus Status
        {
            get
            {
                return data.Conflict.Status switch
                {
                    "pending" => ConflictStatus.Pending,
                    "active" => ConflictStatus.Active,
                    _ => ConflictStatus.Concluded,
                };
            }
        }

        public FactionConflict Faction1 => new(data.Conflict.Faction1, FactionWarStatus(data.Conflict.Faction1.WonDays, data.Conflict.Faction2.WonDays));

        public string Score { get; } = $"{data.Conflict.Faction1.WonDays} vs {data.Conflict.Faction2.WonDays}";

        public FactionConflict Faction2 => new(data.Conflict.Faction2, FactionWarStatus(data.Conflict.Faction2.WonDays, data.Conflict.Faction1.WonDays));

        private static FactionConflictStatus FactionWarStatus(int wonDays, int lostDays)
        {
            var score = wonDays - lostDays;

            switch (score)
            {
                case var sc when sc > 0:
                    return FactionConflictStatus.Winning;
                case var sc when sc < 0:
                    return FactionConflictStatus.Losing;
                default:
                    return FactionConflictStatus.Draw;
            }
        }
    }
}
