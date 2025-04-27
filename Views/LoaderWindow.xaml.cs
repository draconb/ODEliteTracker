using ODEliteTracker.ViewModels;
using ODMVVM.Views;

namespace ODEliteTracker.Views
{
    /// <summary>
    /// Interaction logic for LoaderWindow.xaml
    /// </summary>
    public partial class LoaderWindow : ODWindowBase
    {
        public LoaderWindow()
        {
            InitializeComponent();
            Loaded += LoaderWindow_Loaded;
        }

        private void LoaderWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is LoaderViewModel viewModel)
            {
                viewModel.InitialiseComplete += OnInitialised;
            }
        }

        private void OnInitialised(object? sender, bool e)
        {
            DialogResult = e;
        }
    }
}
