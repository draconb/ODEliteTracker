using ODEliteTracker.Models;
using ODEliteTracker.Themes;
using ODEliteTracker.ViewModels;
using ODJournalDatabase.Database.DTOs;
using ODJournalDatabase.Database.Interfaces;
using ODMVVM.Navigation;

namespace ODEliteTracker.Stores
{
    public sealed class SettingsStore
    {
        public SettingsStore(IODDatabaseProvider databaseProvider, ThemeManager themeManager, IODNavigationService navigationService)
        {
            this.databaseProvider = databaseProvider;
            this.themeManager = themeManager;
            this.navigationService = navigationService;

            this.navigationService.CurrentViewChanged += NavigationService_CurrentViewChanged;
        }

        private void NavigationService_CurrentViewChanged(object? sender, ODMVVM.ViewModels.ODViewModel? e)
        {
            if (e == null)
            {
                return;
            }
            if(e is not LoadingViewModel)
            {
                CurrentViewModel = e.GetType();
            }
        }

        private readonly IODDatabaseProvider databaseProvider;
        private readonly ThemeManager themeManager;
        private readonly IODNavigationService navigationService;

        public int SelectedCommanderID { get; set; } = 0;
        public Type CurrentViewModel { get; set; } = typeof(MassacreMissionsViewModel);
        public Theme CurrentTheme { get; set; } = Theme.OD;
        public JournalLogAge JournalAge { get; set; } = JournalLogAge.OneHundredEightyDays;
        public DateTime JournalAgeDateTime
        {
            get
            {
                return JournalAge switch
                {
                    JournalLogAge.All => DateTime.MinValue,
                    JournalLogAge.SevenDays => DateTime.UtcNow.AddDays(-7),
                    JournalLogAge.ThirtyDays => DateTime.UtcNow.AddDays(-30),
                    JournalLogAge.SixtyDays => DateTime.UtcNow.AddDays(-60),
                    JournalLogAge.OneHundredEightyDays => DateTime.UtcNow.AddDays(-180),
                    _ => DateTime.UtcNow.AddYears(-((int)JournalAge - 4)),
                };
            }
        }

        public CommoditySorting ColonisationCommoditySorting { get; internal set; } = CommoditySorting.Category;

        public void LoadSettings()
        {
            var settings = databaseProvider.GetAllSettings();

            if (settings != null && settings.Count != 0)
            {
                SelectedCommanderID = SettingsDTOHelpers.SettingsDtoToInt(settings.GetSettingDTO(nameof(SelectedCommanderID)));
                CurrentTheme = SettingsDTOHelpers.SettingDtoToEnum(settings.GetSettingDTO(nameof(CurrentTheme)), Theme.OD);
                ColonisationCommoditySorting = SettingsDTOHelpers.SettingDtoToEnum(settings.GetSettingDTO(nameof(ColonisationCommoditySorting)), CommoditySorting.Category);
                CurrentViewModel = SettingsDTOHelpers.SettingDtoToObject(settings.GetSettingDTO(nameof(CurrentViewModel)), typeof(ColonisationViewModel));
            }

            //Apply Theme
            themeManager.SetTheme(CurrentTheme);
        }

        public void SaveSettings()
        {
            var settings = new List<SettingsDTO>
            {
                //Just in case someone closes the app while scanning a new directory
                SettingsDTOHelpers.IntToSettingsDTO(nameof(SelectedCommanderID), SelectedCommanderID > 0 ? SelectedCommanderID : 0),
                SettingsDTOHelpers.EnumToSettingsDto(nameof(CurrentTheme), CurrentTheme),
                SettingsDTOHelpers.EnumToSettingsDto(nameof(ColonisationCommoditySorting), ColonisationCommoditySorting),
                SettingsDTOHelpers.ObjectToJsonStringDto(nameof(CurrentViewModel), CurrentViewModel)
            };

            databaseProvider.AddSettings(settings);
        }
    }
}
