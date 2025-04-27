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

        private string statusText = "Reading History";
        public string StatusText
        {
            get => statusText;
            set
            {
                statusText = value;
                OnPropertyChanged(nameof(StatusText));
            }
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
