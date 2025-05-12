
namespace ODEliteTracker.Models.Settings
{
    public sealed class MassacreSettings
    {
        public bool HideCompletedStacks { get; set; }
        public MissionSorting MissionSorting { get; internal set; }

        public static MassacreSettings GetDefault()
        {
            return new()
            {
                HideCompletedStacks = false
            };
        }
    }
}
