using System.Windows;

namespace ODEliteTracker.Themes
{
    public enum Theme
    {
        OD,
        Dark,
        Light
    }

    public sealed class ThemeDictionary : ResourceDictionary
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

        public void UpdateSource(Theme theme)
        {
            Source = GetTheme(theme);
        }

        private static Uri GetTheme(Theme theme)
        {
            var uri = "pack://application:,,,/ODEliteTracker;component/Themes/DefaultTheme.xaml";

            switch (theme)
            {
                case Theme.Dark:
                    uri = "pack://application:,,,/ODEliteTracker;component/Themes/DarkTheme.xaml";
                    break;
                case Theme.Light:
                    uri = "pack://application:,,,/ODEliteTracker;component/Themes/LightTheme.xaml";
                    break;
                case Theme.OD:
                default:
                    break;
            }
            return new Uri(uri, UriKind.Absolute);
        }
    }
}
