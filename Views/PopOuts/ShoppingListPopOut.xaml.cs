using System.Windows.Controls;

namespace ODEliteTracker.Views.PopOuts
{
    /// <summary>
    /// Interaction logic for ShoppingListPopOut.xaml
    /// </summary>
    public partial class ShoppingListPopOut : UserControl
    {
        public ShoppingListPopOut()
        {
            InitializeComponent();
            Loaded += ShoppingListPopOut_Loaded;
            Unloaded += ShoppingListPopOut_Unloaded;
        }

        private void ShoppingListPopOut_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is ViewModels.PopOuts.ShoppingListPopOutVM vm)
            {
                vm.Settings.ColumnVisibilityChanged += OnVisibilityChanged;
                OnVisibilityChanged(null, EventArgs.Empty);
            }
        }

        private void ShoppingListPopOut_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is ViewModels.PopOuts.ShoppingListPopOutVM vm)
            {
                vm.Settings.ColumnVisibilityChanged -= OnVisibilityChanged;
            }
        }

        private void OnVisibilityChanged(object? sender, EventArgs e)
        {
            if (ResourceGrid is null || DataContext is not ViewModels.PopOuts.ShoppingListPopOutVM vm)
                return;

            ResourceGrid.Columns[0].Visibility = vm.Settings.NameVis;
            ResourceGrid.Columns[0].Width = vm.Settings.CategoryVis
                                            == System.Windows.Visibility.Visible ? new DataGridLength(1, DataGridLengthUnitType.Auto) 
                                            : new DataGridLength(1, DataGridLengthUnitType.Star);
            ResourceGrid.Columns[1].Visibility = vm.Settings.CategoryVis;
            ResourceGrid.Columns[2].Visibility = vm.Settings.MarketStockVis;
            ResourceGrid.Columns[3].Visibility = vm.Settings.CarrierStockVis;
            ResourceGrid.Columns[4].Visibility = vm.Settings.CarrierDiffVis;
            ResourceGrid.Columns[5].Visibility = vm.Settings.RemainingVis;
        }
    }
}
