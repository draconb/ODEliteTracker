using ToastNotifications.Core;

namespace ODEliteTracker.Notifications
{
    /// <summary>
    /// Interaction logic for BasicNotificationPart.xaml
    /// </summary>
    public partial class BasicNotificationPart : NotificationDisplayPart
    {
        public BasicNotificationPart(BasicNotification notification)
        {
            InitializeComponent();
            Bind(notification);
        }
    }
}
