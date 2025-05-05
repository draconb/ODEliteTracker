using ODMVVM.ViewModels;
using System.Windows;

namespace ODEliteTracker.Notifications.Themes
{
    public sealed class NotificationThemeManager : ODObservableObject
    {
        private NotificationTheme currentTheme;
        public NotificationTheme CurrentTheme
        {
            get => currentTheme;
            set
            {
                currentTheme = value;
                OnPropertyChanged(nameof(CurrentTheme));
            }
        }

        public void SetTheme(NotificationTheme theme)
        {
            var themeDict = (NotificationThemeDictionary?)Application.Current.Resources.MergedDictionaries.Where(x => x is NotificationThemeDictionary).FirstOrDefault();

            if (themeDict != null)
            {
                CurrentTheme = theme;
                themeDict.UpdateSource(theme);
            }
        }
    }
}
