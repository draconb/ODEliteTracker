using ToastNotifications.Core;

namespace ODEliteTracker.Notifications.Test
{
    /// <summary>
    /// Interaction logic for TestNotificationPart.xaml
    /// </summary>
    public partial class TestNotificationPart : NotificationDisplayPart
    {
        public TestNotificationPart(TestNotification testNotification)
        {
            InitializeComponent();
            Bind(testNotification);
        }
    }
}
