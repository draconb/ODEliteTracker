using System.Windows.Controls;

namespace ODEliteTracker.Views.PopOuts
{
    /// <summary>
    /// Interaction logic for ColonisationPopOut.xaml
    /// </summary>
    public partial class ColonisationPopOut : UserControl
    {
        public ColonisationPopOut()
        {
            InitializeComponent();
            Loaded += ColonisationPopOut_Loaded;
            Unloaded += ColonisationPopOut_Unloaded;
        }

        private void ColonisationPopOut_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is ViewModels.PopOuts.ColonisationPopOut vm)
            {
                vm.Settings.ColumnVisibilityChanged += OnVisibilityChanged;
                OnVisibilityChanged(null, EventArgs.Empty);
            }
        }

        private void ColonisationPopOut_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is ViewModels.PopOuts.ColonisationPopOut vm)
            {
                vm.Settings.ColumnVisibilityChanged -= OnVisibilityChanged;
            }
        }

        private void OnVisibilityChanged(object? sender, EventArgs e)
        {
            if (ResourceGrid is null || DataContext is not ViewModels.PopOuts.ColonisationPopOut vm)
                return;

            ResourceGrid.Columns[0].Visibility = vm.Settings.NameVis;
            ResourceGrid.Columns[0].Width = vm.Settings.CategoryVis 
                == System.Windows.Visibility.Visible ? new DataGridLength(1, DataGridLengthUnitType.Auto) 
                : new DataGridLength(1, DataGridLengthUnitType.Star);
            ResourceGrid.Columns[1].Visibility = vm.Settings.CategoryVis;
            ResourceGrid.Columns[2].Visibility = vm.Settings.MarketStockVis;
            ResourceGrid.Columns[3].Visibility = vm.Settings.CarrierStockVis;
            ResourceGrid.Columns[4].Visibility = vm.Settings.RemainingVis;
        }
    }
}
