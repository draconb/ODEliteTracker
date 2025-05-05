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
        }
    }
}
