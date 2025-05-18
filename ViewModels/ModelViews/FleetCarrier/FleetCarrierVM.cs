using ODMVVM.Extensions;
using ODMVVM.Helpers;
using ODMVVM.ViewModels;

namespace ODEliteTracker.ViewModels.ModelViews.FleetCarrier
{
    public sealed class FleetCarrierVM : ODObservableObject
    {
        public FleetCarrierVM(Models.FleetCarrier.FleetCarrier carrier)
        {
            this.carrier = carrier;
        }

        private Models.FleetCarrier.FleetCarrier carrier;
        public string Name => carrier.Name;
        public string Callsign => carrier.Callsign;
        public string Fuel =>    $"{(int)Math.Round((double)(100 * carrier.FuelLevel) / 1000)}% ({carrier.FuelLevel:N0} t)";
        public string StarSystem => carrier.StarSystem;
        public string Balance => $"{carrier.Balance:N0} cr";

        public string Destination => carrier.Destination.SystemName;
        public string DestinationBody => carrier.Destination.BodyName;
        public string DepartTime => carrier.Destination.DepartureTime.RelativeCarrierTime(DateTime.UtcNow);

        public string Bartender => carrier.Crew[EliteJournalReader.CarrierCrewRole.Bartender].GetEnumDescription();
        public string BlackMarket => carrier.Crew[EliteJournalReader.CarrierCrewRole.BlackMarket].GetEnumDescription();
        public string Outfitting => carrier.Crew[EliteJournalReader.CarrierCrewRole.Outfitting].GetEnumDescription();
        public string PioneerSupplies => carrier.Crew[EliteJournalReader.CarrierCrewRole.PioneerSupplies].GetEnumDescription();
        public string Rearm => carrier.Crew[EliteJournalReader.CarrierCrewRole.Rearm].GetEnumDescription();
        public string Refuel => carrier.Crew[EliteJournalReader.CarrierCrewRole.Refuel].GetEnumDescription();
        public string Repair => carrier.Crew[EliteJournalReader.CarrierCrewRole.Repair].GetEnumDescription();
        public string Shipyard => carrier.Crew[EliteJournalReader.CarrierCrewRole.Shipyard].GetEnumDescription();
        public string VoucherRedemption => carrier.Crew[EliteJournalReader.CarrierCrewRole.VoucherRedemption].GetEnumDescription();
        public string Vista => carrier.Crew[EliteJournalReader.CarrierCrewRole.VistaGenomics].GetEnumDescription();

        public IEnumerable<CarrierCommodityVM> Stock => carrier.Stock.Select(x => new CarrierCommodityVM(x));

        public void UpdateStock(Models.FleetCarrier.FleetCarrier carrier)
        {
            //Update the reference just in case
            this.carrier = carrier;
            OnPropertyChanged(nameof(Stock));
        }

        internal void UpdateTimes()
        {
            OnPropertyChanged(nameof(DepartTime));
        }

        internal void UpdateDestination(Models.FleetCarrier.FleetCarrier e)
        {
            //Update the reference just in case
            this.carrier = e;
            OnPropertyChanged(nameof(StarSystem));
            OnPropertyChanged(nameof(Fuel));
            OnPropertyChanged(nameof(Destination));
            OnPropertyChanged(nameof(DestinationBody));
            OnPropertyChanged(nameof(DepartTime));
        }

        internal void UpdateData(Models.FleetCarrier.FleetCarrier e)
        {
            this.carrier = e;
            OnPropertyChanged(nameof(StarSystem));
            OnPropertyChanged(nameof(Fuel));
            OnPropertyChanged(nameof(Balance));
            OnPropertyChanged(nameof(Destination));
            OnPropertyChanged(nameof(DestinationBody));
            OnPropertyChanged(nameof(DepartTime));
            OnPropertyChanged(nameof(Bartender));
            OnPropertyChanged(nameof(BlackMarket));
            OnPropertyChanged(nameof(Outfitting));
            OnPropertyChanged(nameof(PioneerSupplies));
            OnPropertyChanged(nameof(Rearm));
            OnPropertyChanged(nameof(Refuel));
            OnPropertyChanged(nameof(Repair));
            OnPropertyChanged(nameof(Shipyard));
            OnPropertyChanged(nameof(VoucherRedemption));
            OnPropertyChanged(nameof(Vista));
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Stock));
        }
    }
}
