using ODEliteTracker.Models.Ship;
using ODMVVM.Extensions;
using ODMVVM.ViewModels;
using System.Collections.ObjectModel;

namespace ODEliteTracker.ViewModels.ModelViews
{
    public sealed class ShipInfoVM(ShipInfo info) : ODObservableObject
    {
        public string Name { get; } = info.Name;
        public string Ident { get; } = info.Ident;
        public int CargoCapacity { get; } = info.CargoCapacity;
        public int CargoCount
        {
            get
            {
                return ShipCargo.Sum(x => x.Count);
            }
        }
        public string CargoText
        {
            get
            {
                return $"Cargo {CargoCount} / {CargoCapacity}";
            }
        }
        public string CargoRemaining
        {
            get
            {
                return $"{CargoCapacity - CargoCount} Unused";
            }
        }
        public ObservableCollection<ShipCargoVM> ShipCargo { get; set; } = [];

        internal void OnCargoUpdated(IEnumerable<ShipCargo>? e)
        {
            ShipCargo.ClearCollection();

            if (e != null)
            {
                var cargo = e.Select(x => new ShipCargoVM(x));

                if (cargo.Any())
                {
                    ShipCargo.AddRange(cargo);
                }
            }
            OnPropertyChanged(nameof(CargoText));
            OnPropertyChanged(nameof(CargoRemaining));
        }
    }
}
