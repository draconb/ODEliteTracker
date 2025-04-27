using ODMVVM.ViewModels;
using System.Collections.ObjectModel;

namespace ODEliteTracker.ViewModels.ModelViews.Colonisation
{
    public sealed class CommanderSystemVM(long systemAddress, string systemName) : ODObservableObject
    {
        public long SystemAddress { get; } = systemAddress;
        public string SystemName { get; } = systemName;
        public string SystemNameUpper => SystemName.ToUpper();
        public ObservableCollection<ConstructionDepotVM> Depots { get; } = [];
    }
}
