using System.Windows;

namespace ODEliteTracker.Notifications.Themes
{
    public enum NotificationTheme
    {
        Elite,
        OD,
        Dark,
        Light
    }

    public sealed class NotificationThemeDictionary : ResourceDictionary
    {
        private Uri? _defaultSource;
        public Uri? DefaultSource
        {
            get => _defaultSource;
            set
            {
                _defaultSource = value;
                Source = _defaultSource;
            }
        }

        public void UpdateSource(NotificationTheme theme)
        {
            Source = GetTheme(theme);
        }

        private static Uri GetTheme(NotificationTheme theme)
        {
            var uri = "pack://application:,,,/ODEliteTracker;component/Notifications/Themes/EliteStyleNotificationTheme.xaml";

            switch (theme)
            {
                case NotificationTheme.Dark:
                    uri = "pack://application:,,,/ODEliteTracker;component/Notifications/Themes/DarkNotificationTheme.xaml";
                    break;
                case NotificationTheme.Light:
                    uri = "pack://application:,,,/ODEliteTracker;component/Notifications/Themes/LightNotificationTheme.xaml";
                    break;
                case NotificationTheme.OD:
                    uri = "pack://application:,,,/ODEliteTracker;component/Notifications/Themes/ODNotificationTheme.xaml";
                    break;
                default:
                    break;
            }
            return new Uri(uri, UriKind.Absolute);
        }
    }
}
