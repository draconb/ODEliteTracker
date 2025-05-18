namespace ODEliteTracker.Models.Settings
{
    public sealed class ColonisationSettings
    {
        public event EventHandler<CommoditySorting>? CommoditySortingChanged;

        private CommoditySorting colonisationCommoditySorting = CommoditySorting.Category;
        public CommoditySorting ColonisationCommoditySorting
        {
            get => colonisationCommoditySorting;
            internal set
            {
                if (value != colonisationCommoditySorting)
                {
                    colonisationCommoditySorting = value;
                    CommoditySortingChanged?.Invoke(this, ColonisationCommoditySorting);
                }
            }
        }

        public CommoditySorting ShoppingListSorting { get; set; } = CommoditySorting.Category;

        public int SelectedDepotTab { get; set; }
    }
}
