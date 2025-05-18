using ODEliteTracker.Models.Settings;
using ODEliteTracker.Services;
using ODEliteTracker.Stores;
using ODEliteTracker.Themes.Overlay;
using ODMVVM.Commands;
using ODMVVM.ViewModels;
using System.Windows.Input;

namespace ODEliteTracker.ViewModels
{
    public sealed class PopOutControlViewModel : ODViewModel
    {
        public PopOutControlViewModel(PopOutService popOutService, SettingsStore settings, OverlayThemeManager themeManager)
        {
            this.popOutService = popOutService;
            this.settings = settings;
            this.themeManager = themeManager;

            OpenPopOut = new ODRelayCommand<Type>(OnOpenPopOut);
            SetTheme = new ODRelayCommand<OverlayTheme>(OnSetTheme);
        }

        private readonly PopOutService popOutService;
        private readonly SettingsStore settings;
        private readonly OverlayThemeManager themeManager;

        public override bool IsLive => true;

        public ICommand OpenPopOut { get; }
        public ICommand SetTheme { get; }

        public OverlaySettings Settings => settings.OverlaySettings;

        private void OnOpenPopOut(Type type)
        {
            popOutService.OpenPopOut(type, settings.SelectedCommanderID);
        }

        private void OnSetTheme(OverlayTheme theme)
        {
            settings.OverlaySettings.CurrentTheme = theme;
            themeManager.SetTheme(theme);
        }
    }
}
