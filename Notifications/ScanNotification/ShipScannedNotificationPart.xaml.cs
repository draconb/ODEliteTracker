using ToastNotifications.Core;

namespace ODEliteTracker.Notifications.ScanNotification
{
    /// <summary>
    /// Interaction logic for ShipScannedNotificationPart.xaml
    /// </summary>
    public partial class ShipScannedNotificationPart : NotificationDisplayPart
    {
        public ShipScannedNotificationPart(ShipScannedNotification notification)
        {
            InitializeComponent();
            Bind(notification);
            Unloaded += ShipScannedNotificationPart_Unloaded;
        }

        private void ShipScannedNotificationPart_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if(DataContext is ShipScannedNotification notification)
            {
                notification.Close();
            }
        }
    }
}
