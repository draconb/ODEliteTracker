using Microsoft.EntityFrameworkCore;
using ODEliteTracker.Database;
using ODJournalDatabase.JournalManagement;
using ODMVVM.ViewModels;

namespace ODEliteTracker.ViewModels
{
    public sealed class LoadingViewModel : ODViewModel
    {
        public LoadingViewModel(ODEliteTrackerDbContextFactory contextFactory,
                                         JournalEventParser eventParser)
        {
            this.contextFactory = contextFactory;
            this.eventParser = eventParser;

            this.eventParser.OnReadingNewFile += EventParser_OnReadingNewFile;
        }
        public override bool IsLive => false;
        private readonly ODEliteTrackerDbContextFactory contextFactory;
        private readonly JournalEventParser eventParser;

        private string? statusText;
        public string? StatusText
        {
            get => statusText;
            set
            {
                statusText = value;
                OnPropertyChanged(nameof(StatusText));
            }
        }

        public async Task Initialise()
        {
            await Task.Delay(500);
            StatusText = "Loading";
            await Task.Delay(500);
            StatusText = "Migrating Database";
            try
            {
                using var dbContext = contextFactory.CreateDbContext();
                await dbContext.Database.MigrateAsync();
                await Task.Delay(500);
            }
            catch (Microsoft.Data.Sqlite.SqliteException ex)
            {
                StatusText = "Error Accessing Database\nApplication will now close";
                await Task.Delay(5000);
                App.Current.Shutdown();
            }
            catch (Exception ex)
            {
                StatusText ="Error Loading\nApplication will now close";
                await Task.Delay(5000);
                App.Current.Shutdown();
            }
            await Task.Delay(500);
            StatusText = "Reading History";
            await Task.Delay(500);
        }

        private void EventParser_OnReadingNewFile(object? sender, string e)
        {
            StatusText = e;
        }

        public override void Dispose()
        {
            this.eventParser.OnReadingNewFile -= EventParser_OnReadingNewFile;
        }
    }
}
