using ODMVVM.Navigation.Controls;
using System.Windows.Input;

namespace ODEliteTracker.Controls.Navigation
{
    /// <summary>
    /// Interaction logic for UiScaleButton.xaml
    /// </summary>
    public partial class UiScaleButton : UtilNavigationButton
    {
        public UiScaleButton()
        {
            InitializeComponent();
        }

        private void Popup_Closed(object sender, EventArgs e)
        {
            if (!object.Equals(this.button, Mouse.DirectlyOver))
            {
                this.button.IsChecked = false;
            }
        }

        private void Popup_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.button.IsChecked = false;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(DataContext is ODNavigationView view)
            {
                view.UiScale = 1;
            }
        }
    }
}
