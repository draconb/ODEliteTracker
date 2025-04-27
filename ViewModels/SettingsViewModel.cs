using ODEliteTracker.Models;
using ODEliteTracker.Stores;
using ODEliteTracker.Themes;
using ODEliteTracker.ViewModels.ModelViews;
using ODJournalDatabase.Database.Interfaces;
using ODJournalDatabase.JournalManagement;
using ODMVVM.Commands;
using ODMVVM.Extensions;
using ODMVVM.Navigation;
using ODMVVM.Services.MessageBox;
using ODMVVM.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ODEliteTracker.ViewModels
{
    public sealed class SettingsViewModel : ODViewModel
    {
        public SettingsViewModel(ThemeManager themeManager,
                                 SettingsStore setting,
                                 IODNavigationService navigationService,
                                 IODDatabaseProvider databaseProvider,
                                 IManageJournalEvents journalManager,
                                 IODDialogService messageBoxService)
        {
            this.themeManager = themeManager;
            this.setting = setting;
            this.navigationService = navigationService;
            this.databaseProvider = databaseProvider;
            this.journalManager = journalManager;
            this.dialogService = messageBoxService;
            SetTheme = new ODRelayCommand<Theme>(OnChangeTheme);
            OpenUrlCommand = new ODRelayCommand<string>(OpenUrl);

            ToggleCommanderHidden = new ODRelayCommand(OnToggleCommanderHidden, (_) => SelectedCommander != null && SelectedCommander?.Id != setting.SelectedCommanderID);
            ResetLastReadFile = new ODRelayCommand(OnResetLastFile, (_) => SelectedCommander != null && SelectedCommander?.Id != setting.SelectedCommanderID);
            ChangeJourneyDirectoryCommand = new ODAsyncRelayCommand(OnChangeJournalDirectory, () => SelectedCommander != null && SelectedCommander?.Id != setting.SelectedCommanderID);
            SaveCommanderChanges = new ODAsyncRelayCommand(OnSaveCommanderChanges, () => SelectedCommander != null);
            ReadNewDirectoryCommand = new ODAsyncRelayCommand(OnReadNewDirectory);
            DeleteCommander = new ODAsyncRelayCommand(OnDeleteCommander, () => SelectedCommander?.Id != setting.SelectedCommanderID);
            ResetDataBaseCommand = new ODAsyncRelayCommand(OnResetDataBase);

            _ = LoadCommanders();
            OnModelLive(true);
        }

        private readonly ThemeManager themeManager;
        private readonly SettingsStore setting;
        private readonly IODNavigationService navigationService;
        private readonly IODDatabaseProvider databaseProvider;
        private readonly IManageJournalEvents journalManager;
        private readonly IODDialogService dialogService;

        public override bool IsLive => true;
        public Theme CurrentTheme => setting.CurrentTheme;
        public JournalLogAge LogAge
        {
            get
            {
                return setting.JournalAge;
            }
            set
            {
                setting.JournalAge = value;
                _ = ChangeJournalAge();
            }
        }
        
        private ObservableCollection<JournalCommanderVM> journalCommanderViewModelCollection = [];
        public ObservableCollection<JournalCommanderVM> JournalCommanderViews
        {
            get => journalCommanderViewModelCollection;
            set => journalCommanderViewModelCollection = value;
        }

        private JournalCommanderVM? selectedCommander;
        public JournalCommanderVM? SelectedCommander
        {
            get => selectedCommander;
            set
            {
                selectedCommander = value;
                OnPropertyChanged(nameof(SelectedCommander));
            }
        }

        #region Commands
        public ICommand SetTheme { get; }
        public ICommand OpenUrlCommand { get; }
        public ICommand ToggleCommanderHidden { get; }
        public ICommand ResetLastReadFile { get; }
        public ICommand ChangeJourneyDirectoryCommand { get; }
        public ICommand SaveCommanderChanges { get; }
        public ICommand DeleteCommander { get; }
        public ICommand ReadNewDirectoryCommand { get; }
        public ICommand ResetDataBaseCommand { get; }
        #endregion

        #region Command Methods
        private void OnChangeTheme(Theme theme)
        {
            setting.CurrentTheme = theme;
            themeManager.SetTheme(theme);
            OnPropertyChanged(nameof(CurrentTheme));
        }

        private void OpenUrl(string? url)
        {
            if (string.IsNullOrEmpty(url))
                return;
            ODMVVM.Helpers.OperatingSystem.OpenUrl(url);
        }

        private void OnToggleCommanderHidden(object? obj)
        {
            if (SelectedCommander != null)
                SelectedCommander.IsHidden = !SelectedCommander.IsHidden;
        }

        private async Task OnChangeJournalDirectory()
        {
            if (SelectedCommander == null)
            {
                return;
            }
            var directory = dialogService.DirectorySelectDialog(SelectedCommander.JournalPath);

            if (string.IsNullOrEmpty(directory) == false && SelectedCommander != null)
            {
                SelectedCommander.JournalPath = directory;
                SelectedCommander.LastFile = string.Empty;
                await OnSaveCommanderChanges();
            }
        }

        private void OnResetLastFile(object? obj)
        {
            if (SelectedCommander != null)
                SelectedCommander.LastFile = string.Empty;
        }

        private async Task OnDeleteCommander()
        {
            if (SelectedCommander is null)
                return;

            var result = dialogService.ShowWithOwner(null, "Delete CMDR?", $"Delete CMDR {SelectedCommander.Name}?", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                navigationService.NavigateTo<LoadingViewModel>();
                await Task.Factory.StartNew(() => databaseProvider.DeleteCommander(SelectedCommander.Id)).ConfigureAwait(true);
                await journalManager.UpdateCommanders();
                navigationService.NavigateTo<SettingsViewModel>();
            }
        }

        private async Task OnReadNewDirectory()
        {
            var directory = dialogService.DirectorySelectDialog();

            if (string.IsNullOrEmpty(directory) == false)
            {
                navigationService.NavigateTo<LoadingViewModel>();
                await journalManager.ReadNewDirectory(directory);
                navigationService.NavigateTo<SettingsViewModel>();
            }
        }

        private async Task OnSaveCommanderChanges()
        {
            if (SelectedCommander == null)
                return;

            foreach (var commander in JournalCommanderViews)
            {
                databaseProvider.AddCommander(new(commander.Id, commander.Name, commander.JournalPath, commander.LastFile, commander.IsHidden));
            }
            await journalManager.UpdateCommanders();
        }

        private async Task OnResetDataBase()
        {
            JournalCommanderViews.ClearCollection();
            SelectedCommander = null;
            OnPropertyChanged(nameof(JournalCommanderViews));
            OnPropertyChanged(nameof(SelectedCommander));
            setting.SelectedCommanderID = 0;
            navigationService.NavigateTo<LoadingViewModel>();
            if(navigationService.CurrentView is LoadingViewModel viewModel)
            {
                viewModel.StatusText = "Resetting Database...";
                await journalManager.ResetDatabase();
            }
            navigationService.NavigateTo<SettingsViewModel>();
        }
        #endregion

        private async Task ChangeJournalAge()
        {
            navigationService.NavigateTo<LoadingViewModel>();
            await journalManager.ChangeCommander();
            navigationService.NavigateTo<SettingsViewModel>();
        }

        private async Task LoadCommanders()
        {
            var commanders = await databaseProvider.GetAllJournalCommanders(true);

            var vms = commanders.Select(x => new JournalCommanderVM(x));

            JournalCommanderViews.ClearCollection();

            foreach (var commander in vms)
            {
                JournalCommanderViews.AddItem(commander);
            }

            SelectedCommander = JournalCommanderViews.FirstOrDefault(x => x.Id == setting.SelectedCommanderID);
            OnPropertyChanged(nameof(SelectedCommander));
            OnPropertyChanged(nameof(JournalCommanderViews));
        }
    }
}
