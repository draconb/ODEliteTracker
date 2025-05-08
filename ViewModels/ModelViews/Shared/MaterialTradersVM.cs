using ODEliteTracker.Models.Galaxy;
using ODEliteTracker.Services;
using ODMVVM.ViewModels;

namespace ODEliteTracker.ViewModels.ModelViews.Shared
{
    public sealed class MaterialTradersVM : ODObservableObject
    {
        public IEnumerable<MaterialTraderVM> ManufacturedTraders { get; private set; } = [];
        public IEnumerable<MaterialTraderVM> EncodedTraders { get; private set; } = [];
        public IEnumerable<MaterialTraderVM> RawTraders { get; private set; } = [];

        public void PopulateTraders(Tuple<IEnumerable<MaterialTrader>, IEnumerable<MaterialTrader>, IEnumerable<MaterialTrader>> traders, 
                                    Position currentPos)
        {
            ManufacturedTraders = traders.Item1.Select(x => new MaterialTraderVM(x, currentPos));
            EncodedTraders = traders.Item2.Select(x => new MaterialTraderVM(x, currentPos));
            RawTraders = traders.Item3.Select(x => new MaterialTraderVM(x, currentPos));

            OnPropertyChanged(nameof(ManufacturedTraders));
            OnPropertyChanged(nameof(EncodedTraders));
            OnPropertyChanged(nameof(RawTraders));
        }
    }
}
