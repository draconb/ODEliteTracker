using ODEliteTracker.Models.Galaxy;
using ODEliteTracker.Services;
using ODMVVM.Commands;
using System.Windows.Input;

namespace ODEliteTracker.ViewModels.ModelViews.Shared
{
    public sealed class MaterialTraderVM(MaterialTrader trader, Position currentPos)
    {
        public string SystemName { get; } = trader.SystemName;
        public string StationName { get; } = trader.StationName;
        public string DistanceToArrival { get; } =$"{trader.DistanceToArrival:N0} ls";
        public string Economy { get; } = trader.Economy;
        public string Distance { get; } = $"{Math.Abs(Position.Distance(currentPos, trader.Position)):N2} ly";

        public ICommand CopyToClipboardCommand { get; } = new ODRelayCommand<string>(OnCopyToClipboard);

        private static void OnCopyToClipboard(string name)
        {
            ODMVVM.Helpers.OperatingSystem.SetStringToClipboard(name);
        }
    }
}
