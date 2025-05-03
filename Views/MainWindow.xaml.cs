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
        private bool loaded;
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
                WindowState = viewModel.WindowPosition.State;
                base.WindowBase_Loaded(sender, e);
                viewModel.WindowPositionReset += OnWindowPositionReset;
                await viewModel.Initialise();
            }

            loaded = true;           
        }

        private void OnWindowPositionReset(object? sender, EventArgs e)
        {
            WindowState = System.Windows.WindowState.Normal;
        }

        protected override void StateChangeRaised(object? sender, EventArgs e)
        {
            if (loaded && DataContext is MainViewModel vm)
            {
                vm.WindowPosition.State = WindowState;
            }
            base.StateChangeRaised(sender, e);
        }
    }
}