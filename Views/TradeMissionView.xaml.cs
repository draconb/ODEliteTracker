using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ODEliteTracker.Views
{
    /// <summary>
    /// Interaction logic for TradeMissionView.xaml
    /// </summary>
    public partial class TradeMissionView : UserControl
    {
        public TradeMissionView()
        {
            InitializeComponent();
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                {
                    RoutedEvent = MouseWheelEvent,
                    Source = sender
                };
                var parent = ((Control)sender).Parent as UIElement;
                parent?.RaiseEvent(eventArg);
            }
        }
    }
}
