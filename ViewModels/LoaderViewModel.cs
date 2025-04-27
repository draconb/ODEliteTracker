using Microsoft.EntityFrameworkCore;
using ODEliteTracker.Database;
using ODMVVM.Helpers.IO;
using ODMVVM.Models;
using ODMVVM.Services.MessageBox;
using ODMVVM.ViewModels;

namespace ODEliteTracker.ViewModels
{
    public sealed class LoaderViewModel : ODObservableObject
    {
        public LoaderViewModel(ODEliteTrackerDbContextFactory contextFactory,
                               IODDialogService dialogService)
        {
            this.contextFactory = contextFactory;
            this.dialogService = dialogService;
            _ = Initialise();
        }

        private readonly ODEliteTrackerDbContextFactory contextFactory;
        private readonly IODDialogService dialogService;
        private string statusText = "Loading";
        public string StatusText
        {
            get => statusText;
            set
            {
                statusText = value;
                OnPropertyChanged(nameof(StatusText));
            }
        }

        public EventHandler<bool>? InitialiseComplete;

        public async Task Initialise()
        {
            await Task.Delay(1000);
            StatusText = "Checking For App Updates";
            await Task.Delay(1000);

            try
            {
                var updateInfo = await Json.GetJsonFromUrlAndDeserialise<UpdateInfo>("https://raw.githubusercontent.com", "/WarmedxMints/ODUpdates/main/ODEliteTrackerUpdate.json");

                if (updateInfo.Version > App.AppVersion)
                {
                    var update = dialogService.ShowWithOwner(null, $"Version {updateInfo.Version} is available", "Would you like to download?", System.Windows.MessageBoxButton.YesNo);

                    if(update == System.Windows.MessageBoxResult.OK)
                    {
                        ODMVVM.Helpers.OperatingSystem.OpenUrl(updateInfo.Url);
                    }                    
                }

                if(updateInfo.Version == App.AppVersion)
                {
                    StatusText = "Already on latest version";
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                StatusText = "Error Getting Update";
                await Task.Delay(1000);
            }
            StatusText = "Migrating Database";
            try
            {
                using var dbContext = contextFactory.CreateDbContext();
                await dbContext.Database.MigrateAsync();
                await Task.Delay(1000);
            }
            catch (Microsoft.Data.Sqlite.SqliteException ex)
            {
                StatusText = "Error Accessing Database\nApplication will now close";
                await Task.Delay(2000);
                InitialiseComplete?.Invoke(this, false);
            }
            catch (Exception ex)
            {
                StatusText = "Error Loading\nApplication will now close";
                await Task.Delay(2000);
                InitialiseComplete?.Invoke(this, false);
            }

            InitialiseComplete?.Invoke(this, true);
        }
    }
}
