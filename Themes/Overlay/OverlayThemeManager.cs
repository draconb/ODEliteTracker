using ODMVVM.ViewModels;
using System.Windows;

namespace ODEliteTracker.Themes.Overlay
{
    public sealed class OverlayThemeManager : ODObservableObject
    {
            private OverlayTheme currentTheme;
            public OverlayTheme CurrentTheme
            {
                get => currentTheme;
                set
                {
                    currentTheme = value;
                    OnPropertyChanged(nameof(CurrentTheme));
                }
            }

            public void SetTheme(OverlayTheme theme)
            {
                var themeDict = (OverlayThemeDictionary?)Application.Current.Resources.MergedDictionaries.Where(x => x is OverlayThemeDictionary).FirstOrDefault();

                if (themeDict != null)
                {
                    CurrentTheme = theme;
                    themeDict.UpdateSource(theme);
                }
            }
        }
}
