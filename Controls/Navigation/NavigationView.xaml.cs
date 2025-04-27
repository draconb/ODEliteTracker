using ODMVVM.Navigation.Controls;
using System.Windows;
using System.Windows.Controls;

namespace ODEliteTracker.Controls.Navigation
{
    /// <summary>
    /// Interaction logic for NavigationView.xaml
    /// </summary>
    public partial class NavigationView : ODNavigationView
    {
        public NavigationView()
        {
            DataContext = this;
            InitializeComponent();
        }

        private void NavigationBtn_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button btn && btn.DataContext is ODNavigationButton odBtn &&  odBtn.TargetView != null)
            {
                NavigateTo(odBtn.TargetView);
            }
        }
    }
}
