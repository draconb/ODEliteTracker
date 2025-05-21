using ODJournalDatabase.JournalManagement;
using ODMVVM.Commands;
using ODMVVM.ViewModels;
using System.Windows.Input;

namespace ODEliteTracker.ViewModels
{
    public sealed class LoadingViewModel : ODViewModel
    {
        public LoadingViewModel(JournalEventParser eventParser)
        {
            this.eventParser = eventParser;

            this.eventParser.OnReadingNewFile += EventParser_OnReadingNewFile;

            OpenUrlCommand = new ODRelayCommand<string>(OpenUrl);
        }

        public override bool IsLive => false;
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
        public ICommand OpenUrlCommand { get; }

        private void OpenUrl(string? url)
        {
            if (string.IsNullOrEmpty(url))
                return;
            ODMVVM.Helpers.OperatingSystem.OpenUrl(url);
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
