using EliteJournalReader.Events;
using ODEliteTracker.Models.Galaxy;

namespace ODEliteTracker.Models.BGS
{
    public sealed class BGSStarSystem : StarSystem
    {
        public BGSStarSystem(LocationEvent.LocationEventArgs evt) : base(evt)
        {
            TickData = [new(evt)];
        }

        public BGSStarSystem(FSDJumpEvent.FSDJumpEventArgs evt) : base(evt)
        {
            TickData = [new(evt)];
            VisitCount = 1;
        }

        public BGSStarSystem(CarrierJumpEvent.CarrierJumpEventArgs evt) : base(evt)
        {
            TickData = [new(evt)];
            VisitCount = 1;
        }

        public int VisitCount { get; private set; }
        public List<SystemTickData> TickData { get; }
        public List<VoucherClaim> VoucherClaims { get; set; } = [];

        public void AddTickData(BGSStarSystem sys)
        {
            VisitCount += sys.VisitCount;

            var data = sys.TickData.First();

            var newData = TickData.Last().NewData(data);

            if (newData == false)
                return;
        
            TickData.Add(data);
        }

        public BGSTickSystem? GetBGSTickSystem(TickData data)
        {
            var tickData = TickData.Where(x => x.VisitedDuringPeriod(data.From, data.To)).LastOrDefault();

            if (tickData == null) 
                return null;

            var claims = VoucherClaims.Where(x => data.TimeWithinTick(x.TimeClaimed));

            return new BGSTickSystem(this,  tickData, claims);
        }
    }
}
