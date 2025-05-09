using ODEliteTracker.Models.Galaxy;
using ODEliteTracker.Services;
using ODMVVM.Commands;
using ODMVVM.ViewModels;
using System.Windows.Input;

namespace ODEliteTracker.ViewModels.ModelViews.Shared
{
    public sealed class MaterialTradersVM : ODObservableObject
    {
        public MaterialTradersVM(NotificationService notification)
        {
            this.notification = notification;
            CopyToClipboardCommand = new ODRelayCommand<string>(OnCopyToClipboard);
        }

        private readonly NotificationService notification;

        private ICommand CopyToClipboardCommand { get; }
        public IEnumerable<MaterialTraderVM> ManufacturedTraders { get; private set; } = [];
        public IEnumerable<MaterialTraderVM> EncodedTraders { get; private set; } = [];
        public IEnumerable<MaterialTraderVM> RawTraders { get; private set; } = [];

        public void PopulateTraders(Tuple<IEnumerable<MaterialTrader>, IEnumerable<MaterialTrader>, IEnumerable<MaterialTrader>> traders, 
                                    Position currentPos)
        {
            ManufacturedTraders = traders.Item1.Select(x => new MaterialTraderVM(x, currentPos, CopyToClipboardCommand));
            EncodedTraders = traders.Item2.Select(x => new MaterialTraderVM(x, currentPos, CopyToClipboardCommand));
            RawTraders = traders.Item3.Select(x => new MaterialTraderVM(x, currentPos, CopyToClipboardCommand));

            OnPropertyChanged(nameof(ManufacturedTraders));
            OnPropertyChanged(nameof(EncodedTraders));
            OnPropertyChanged(nameof(RawTraders));
        }

        private void OnCopyToClipboard(string name)
        {
            if(ODMVVM.Helpers.OperatingSystem.SetStringToClipboard(name))
            {
                notification.ShowBasicNotification(new("Clipboard", [name, "Copied To Clipboard"], Models.Settings.NotificationOptions.CopyToClipboard));
            }
        }
    }
}
