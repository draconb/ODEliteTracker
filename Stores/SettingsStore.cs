using ODEliteTracker.Models;
using ODEliteTracker.Models.Settings;
using ODEliteTracker.Notifications.Themes;
using ODEliteTracker.Themes;
using ODEliteTracker.ViewModels;
using ODJournalDatabase.Database.DTOs;
using ODJournalDatabase.Database.Interfaces;
using ODMVVM.Navigation;
using ODMVVM.ViewModels;

namespace ODEliteTracker.Stores
{
    public sealed class SettingsStore
    {
        public SettingsStore(IODDatabaseProvider databaseProvider,
                             ThemeManager themeManager,
                             IODNavigationService navigationService,
                             NotificationThemeManager notificationTheme)
        {
            this.databaseProvider = databaseProvider;
            this.themeManager = themeManager;
            this.navigationService = navigationService;
            this.notificationTheme = notificationTheme;
            this.navigationService.CurrentViewChanged += NavigationService_CurrentViewChanged;
        }

        private void NavigationService_CurrentViewChanged(object? sender, ODViewModel? e)
        {
            if (e == null)
            {
                return;
            }
            if (e is not LoadingViewModel)
            {
                CurrentViewModel = e.GetType();
            }
        }

        private readonly IODDatabaseProvider databaseProvider;
        private readonly ThemeManager themeManager;
        private readonly IODNavigationService navigationService;
        private readonly NotificationThemeManager notificationTheme;

        public int SelectedCommanderID { get; set; } = 0;
        public Type CurrentViewModel { get; set; } = typeof(MassacreMissionsViewModel);
        public Theme CurrentTheme { get; set; } = Theme.OD;
        public JournalLogAge JournalAge { get; set; } = JournalLogAge.OneHundredEightyDays;
        public ODWindowPosition MainWindowPosition { get; set; } = new();
        public NotificationSettings NotificationSettings { get; set; } = NotificationSettings.GetDefault();
        public MassacreSettings MassacreSettings { get; set; } = MassacreSettings.GetDefault();
        public DateTime JournalAgeDateTime
        {
            get
            {
                return JournalAge switch
                {
                    JournalLogAge.All => new DateTime(2014,12,16),
                    JournalLogAge.SevenDays => DateTime.UtcNow.AddDays(-7),
                    JournalLogAge.ThirtyDays => DateTime.UtcNow.AddDays(-30),
                    JournalLogAge.SixtyDays => DateTime.UtcNow.AddDays(-60),
                    JournalLogAge.OneHundredEightyDays => DateTime.UtcNow.AddDays(-180),
                    _ => DateTime.UtcNow.AddYears(-((int)JournalAge - 4)),
                };
            }
        }

        public CommoditySorting ColonisationCommoditySorting { get; internal set; } = CommoditySorting.Category;
        public double UiScale { get; set; } = 1;
        public BGSViewSettings BGSViewSettings { get; internal set; } = new();
        public PowerPlaySettings PowerPlaySettings { get; internal set; } = new();
        public CarrierSettings CarrierSettings { get; internal set; } = new();

        public void LoadSettings()
        {
            var settings = databaseProvider.GetAllSettings();

            if (settings != null && settings.Count != 0)
            {
                SelectedCommanderID = SettingsDTOHelpers.SettingsDtoToInt(settings.GetSettingDTO(nameof(SelectedCommanderID)));
                CurrentTheme = SettingsDTOHelpers.SettingDtoToEnum(settings.GetSettingDTO(nameof(CurrentTheme)), Theme.OD);
                UiScale = SettingsDTOHelpers.SettingsDtoToDouble(settings.GetSettingDTO(nameof(UiScale)), 1);
                ColonisationCommoditySorting = SettingsDTOHelpers.SettingDtoToEnum(settings.GetSettingDTO(nameof(ColonisationCommoditySorting)), CommoditySorting.Category);
                CurrentViewModel = SettingsDTOHelpers.SettingDtoToObject(settings.GetSettingDTO(nameof(CurrentViewModel)), typeof(ColonisationViewModel));
                BGSViewSettings = SettingsDTOHelpers.SettingDtoToObject(settings.GetSettingDTO(nameof(BGSViewSettings)), new BGSViewSettings());
                PowerPlaySettings = SettingsDTOHelpers.SettingDtoToObject(settings.GetSettingDTO(nameof(PowerPlaySettings)), new PowerPlaySettings());
                JournalAge = SettingsDTOHelpers.SettingDtoToEnum(settings.GetSettingDTO(nameof(JournalAge)), JournalLogAge.OneHundredEightyDays);
                MainWindowPosition = SettingsDTOHelpers.SettingDtoToObject(settings.GetSettingDTO(nameof(MainWindowPosition)), MainWindowPosition);
                NotificationSettings = SettingsDTOHelpers.SettingDtoToObject(settings.GetSettingDTO(nameof(NotificationSettings)), NotificationSettings.GetDefault());
                CarrierSettings = SettingsDTOHelpers.SettingDtoToObject(settings.GetSettingDTO(nameof(CarrierSettings)), CarrierSettings.GetDefault());
                MassacreSettings = SettingsDTOHelpers.SettingDtoToObject(settings.GetSettingDTO(nameof(MassacreSettings)), MassacreSettings.GetDefault());
            }

            //Apply Themes
            themeManager.SetTheme(CurrentTheme);
            notificationTheme.SetTheme(NotificationSettings.CurrentTheme);

            if (MainWindowPosition.IsZero)
            {
                ODWindowPosition.ResetWindowPosition(MainWindowPosition);
            }
        }

        public void SaveSettings()
        {
            var settings = new List<SettingsDTO>
            {
                //Just in case someone closes the app while scanning a new directory
                SettingsDTOHelpers.IntToSettingsDTO(nameof(SelectedCommanderID), SelectedCommanderID > 0 ? SelectedCommanderID : 0),
                SettingsDTOHelpers.EnumToSettingsDto(nameof(CurrentTheme), CurrentTheme),
                SettingsDTOHelpers.DoubleToSettingsDTO(nameof(UiScale), UiScale),
                SettingsDTOHelpers.EnumToSettingsDto(nameof(ColonisationCommoditySorting), ColonisationCommoditySorting),
                SettingsDTOHelpers.EnumToSettingsDto(nameof(JournalAge), JournalAge),
                SettingsDTOHelpers.ObjectToJsonStringDto(nameof(CurrentViewModel), CurrentViewModel),
                SettingsDTOHelpers.ObjectToJsonStringDto(nameof(BGSViewSettings), BGSViewSettings),
                SettingsDTOHelpers.ObjectToJsonStringDto(nameof(PowerPlaySettings), PowerPlaySettings),
                SettingsDTOHelpers.ObjectToJsonStringDto(nameof(MainWindowPosition), MainWindowPosition),
                SettingsDTOHelpers.ObjectToJsonStringDto(nameof(NotificationSettings), NotificationSettings),
                SettingsDTOHelpers.ObjectToJsonStringDto(nameof(CarrierSettings), CarrierSettings),
                SettingsDTOHelpers.ObjectToJsonStringDto(nameof(MassacreSettings), MassacreSettings),
            };

            databaseProvider.AddSettings(settings);
        }
    }
}
