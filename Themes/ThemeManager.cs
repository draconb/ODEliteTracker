using ODMVVM.ViewModels;
using System.Windows;

namespace ODEliteTracker.Themes
{
    public sealed class ThemeManager : ODObservableObject
    {
        private Theme currentTheme;
        public Theme CurrentTheme
        {
            get => currentTheme;
            set
            {
                currentTheme = value;
                OnPropertyChanged(nameof(CurrentTheme));
            }
        }

        public void SetTheme(Theme theme)
        {
            var themeDict = (ThemeDictionary?)Application.Current.Resources.MergedDictionaries.Where(x => x is ThemeDictionary).FirstOrDefault();

            if (themeDict != null)
            {
                CurrentTheme = theme;
                themeDict.UpdateSource(theme);
            }
        }
    }
}
