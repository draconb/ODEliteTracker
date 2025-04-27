using ODEliteTracker.ViewModels;
using ODMVVM.Navigation;
using ODMVVM.Views;

namespace ODEliteTracker.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ODWindowBase
    {
        public MainWindow(IODNavigationService oDNavigationService)
        {
            InitializeComponent();
            NavView.AssignNavigation(oDNavigationService);
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                await viewModel.Initialise();
            }            
        }
    }
}