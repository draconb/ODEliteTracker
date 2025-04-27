using EliteJournalReader.Events;
using NetTopologySuite.Geometries;
using ODEliteTracker.Models.Galaxy;

namespace ODEliteTracker.Models.PowerPlay
{
    public class PowerPlaySystem : StarSystem
    {
        public PowerPlaySystem(LocationEvent.LocationEventArgs evt, DateTime cycle) : base(evt)
        {
            CycleData.Add(cycle, new()
            {
                ControllingPower = evt.ControllingPower,
                PowerState = evt.PowerplayState,
                PowerplayStateControlProgress = evt.PowerplayStateControlProgress,
                PowerplayStateReinforcement = evt.PowerplayStateReinforcement,
                PowerplayStateUndermining = evt.PowerplayStateUndermining,
                PowerConflict = evt.PowerplayConflictProgress?.Select(x => x.Copy()).ToList(),
                Powers = evt.Powers?.ToList()
            });
        }

        public PowerPlaySystem(FSDJumpEvent.FSDJumpEventArgs evt, DateTime cycle) : base(evt)
        {
            CycleData.Add(cycle, new()
            {
                ControllingPower = evt.ControllingPower,
                PowerState = evt.PowerplayState,
                PowerplayStateControlProgress = evt.PowerplayStateControlProgress,
                PowerplayStateReinforcement = evt.PowerplayStateReinforcement,
                PowerplayStateUndermining = evt.PowerplayStateUndermining,
                PowerConflict = evt.PowerplayConflictProgress?.Select(x => x.Copy()).ToList(),
                Powers = evt.Powers?.ToList()
            });
        }

        public Dictionary<DateTime, PowerplayCycleData> CycleData = [];

        public void Add_UpdateCycle(DateTime cycle, PowerPlaySystem newSystem)
        {
            var data = newSystem.CycleData[cycle];

            if (CycleData.TryGetValue(cycle, out var kData))
            {
                kData.Update(data);
                return;
            }
            CycleData.Add(cycle, data);
        }
    }
}
