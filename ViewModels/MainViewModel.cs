using ODEliteTracker.Controls.Navigation;
using ODEliteTracker.Models.Galaxy;
using ODEliteTracker.Services;
using ODEliteTracker.Stores;
using ODEliteTracker.ViewModels.ModelViews;
using ODJournalDatabase.JournalManagement;
using ODMVVM.Extensions;
using ODMVVM.Navigation;
using ODMVVM.Navigation.Controls;
using ODMVVM.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ODEliteTracker.ViewModels
{
    public sealed class MainViewModel : ODViewModel
    {
        public MainViewModel(IODNavigationService oDNavigationService,
                                      IManageJournalEvents journalManager,
                                      SharedDataStore sharedDataStore,
                                      SettingsStore settings)
        {
            navigationService = oDNavigationService;
            this.journalManager = journalManager;
            this.sharedData = sharedDataStore;
            this.settings = settings;
            this.sharedData.OnStoreLive += OnStoreLive;
            this.sharedData.OnCurrentSystemChanged += OnCurrentSystemChanged;
            this.sharedData.OnCurrentBody_Station += OnCurrentBody_StationChanged;
            this.journalManager.OnCommandersUpdated += OnCommandersUpdated;
            navigationService.CurrentViewLive += NavigationService_CurrentViewLive;

            //SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged; ;
        }

        //private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        //{
        //    themeManager.SetTheme(settings.CurrentTheme);
        //}

        public override bool IsLive => true;
        private readonly IODNavigationService navigationService;
        private readonly IManageJournalEvents journalManager;
        private readonly SharedDataStore sharedData;
        private readonly SettingsStore settings;

        public ObservableCollection<ODNavigationButton> MenuButtons { get; } =
        [
            new EliteStyleNavigationButton()
            {
                ButtonImage = new BitmapImage(new Uri("/Assets/Icons/assassin.png", UriKind.Relative)),
                TargetView = typeof(MassacreMissionsViewModel)
            },
            new EliteStyleNavigationButton()
            {
                ButtonImage = new BitmapImage(new Uri("/Assets/Icons/trade.png", UriKind.Relative)),
                TargetView = typeof(TradeMissionViewModel)
            },
            new EliteStyleNavigationButton() 
            { 
                ButtonImage = new BitmapImage(new Uri("/Assets/Icons/ColonisationBtn.png", UriKind.Relative)), 
                TargetView = typeof(ColonisationViewModel)
            },
            new EliteStyleNavigationButton()
            {
                ButtonImage = new BitmapImage(new Uri("/Assets/Icons/powerplay.png", UriKind.Relative)),
                TargetView = typeof(PowerPlayViewModel)
            }
        ];

        public ObservableCollection<ODNavigationButton> FooterButtons { get; } =
        [
            new EliteStyleNavigationButton() 
            { 
                ButtonImage = new BitmapImage(new Uri("/Assets/Icons/settings.png", UriKind.Relative)),
                TargetView = typeof(SettingsViewModel), 
            }
        ];

        public ObservableCollection<JournalCommanderVM> JournalCommanders { get; set; } = [];

        private JournalCommanderVM? selectedCommander;
        public JournalCommanderVM? SelectedCommander
        {
            get => selectedCommander;
            set
            {
                if (value == selectedCommander)
                    return;
                selectedCommander = value;
                if (UiEnabled && selectedCommander != null && selectedCommander.Id != settings.SelectedCommanderID)
                {
                    settings.SelectedCommanderID = selectedCommander.Id;
                    _ = ChangeCommander();
                }

                OnPropertyChanged(nameof(SelectedCommander));
            }
        }

        private bool uiEnabled;
        public bool UiEnabled
        {
            get => uiEnabled;
            set
            {
                if(uiEnabled == value) return;
                uiEnabled = value;
                OnPropertyChanged(nameof(UiEnabled));
            }
        }

        public string CurrentSystemName
        {
            get
            {
                return sharedData.CurrentSystem?.Name ?? string.Empty;
            }
        }

        public string CurrentBody_Station
        {
            get
            {
                return sharedData.CurrentBody_Station ?? string.Empty;
            }
        }

        private void NavigationService_CurrentViewLive(object? sender, bool e)
        {
            UiEnabled = e;
        }

        public async Task Initialise()
        {
            UiEnabled = false;
            navigationService.NavigateTo<LoadingViewModel>();
            if (navigationService.CurrentView is LoadingViewModel loadingViewModel)
            {
                await loadingViewModel.Initialise();               
                settings.LoadSettings();
                await Task.Run(journalManager.Initialise).ConfigureAwait(true);
            }
            navigationService.NavigateTo(settings.CurrentViewModel);
            OnPropertyChanged(nameof(CurrentSystemName));
            OnPropertyChanged(nameof(CurrentBody_Station));
        }

        public async Task ChangeCommander()
        {
            UiEnabled = false;
            navigationService.NavigateTo<LoadingViewModel>();
            if (navigationService.CurrentView is LoadingViewModel loadingViewModel)
            {
                loadingViewModel.StatusText = "Reading History";
                await Task.Run(journalManager.ChangeCommander).ConfigureAwait(true);
            }
            navigationService.NavigateTo(settings.CurrentViewModel);
            UiEnabled = true;
            OnPropertyChanged(nameof(CurrentSystemName));
            OnPropertyChanged(nameof(CurrentBody_Station));
        }

        private void OnStoreLive(object? sender, bool e)
        {
            if(e && journalManager is JournalManager manager)
            {
                Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
                {
                    var cmdrs = manager.Commanders.Select(x => new JournalCommanderVM(x));

                    JournalCommanders.ClearCollection();
                    JournalCommanders.AddRange(cmdrs);
                    SelectedCommander = JournalCommanders.FirstOrDefault(x => x.Id == settings.SelectedCommanderID);
                    OnPropertyChanged(nameof(JournalCommanders));
                    OnPropertyChanged(nameof(SelectedCommander));
                }), DispatcherPriority.DataBind);
            }

        }

        private void OnCommandersUpdated(object? sender, EventArgs e)
        {
            OnStoreLive(null, true);
        }

        private void OnCurrentSystemChanged(object? sender, StarSystem? e)
        {
            OnPropertyChanged(nameof(CurrentSystemName));
            OnPropertyChanged(nameof(CurrentBody_Station));
        }

        private void OnCurrentBody_StationChanged(object? sender, string? e)
        {
            OnPropertyChanged(nameof(CurrentBody_Station));
        }
    }
}
