using System.Windows;

namespace ODEliteTracker.Themes.Overlay
{
    public enum OverlayTheme
    {
        ODBright,
        OD,
        Dark,
        Light,
    }

    public sealed class OverlayThemeDictionary : ResourceDictionary
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

        public void UpdateSource(OverlayTheme theme)
        {
            Source = GetTheme(theme);
        }

        private static Uri GetTheme(OverlayTheme theme)
        {
            var uri = "pack://application:,,,/ODEliteTracker;component/Themes/Overlay/ODBrightTheme.xaml";

            switch (theme)
            {
                case OverlayTheme.Dark:
                    uri = "pack://application:,,,/ODEliteTracker;component/Themes/Overlay/DarkTheme.xaml";
                    break;
                case OverlayTheme.Light:
                    uri = "pack://application:,,,/ODEliteTracker;component/Themes/Overlay/LightTheme.xaml";
                    break;
                case OverlayTheme.OD:
                    uri = "pack://application:,,,/ODEliteTracker;component/Themes/Overlay/ODTheme.xaml";
                    break;
                default:
                    break;
            }
            return new Uri(uri, UriKind.Absolute);
        }
    }
}
