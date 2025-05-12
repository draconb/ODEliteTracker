
namespace ODEliteTracker.Models.Settings
{
    public sealed class CarrierSettings
    {
        public CarrierCommoditySorting Sorting { get; set; } = CarrierCommoditySorting.Category;
        public bool AutoStartTimer { get; set; } = true;

        internal static CarrierSettings GetDefault()
        {
            return new()
            {
                Sorting = CarrierCommoditySorting.Category,
                AutoStartTimer = true
            };
        }
    }
}
