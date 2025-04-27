using Microsoft.EntityFrameworkCore;
using ODEliteTracker.Database;
using ODMVVM.ViewModels;

namespace ODEliteTracker.ViewModels
{
    public sealed class LoaderViewModel : ODObservableObject
    {
        public LoaderViewModel(ODEliteTrackerDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;

            _ = Initialise();
        }

        private readonly ODEliteTrackerDbContextFactory contextFactory;

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
            await Task.Delay(1000);
            InitialiseComplete?.Invoke(this, true);
        }
    }
}
