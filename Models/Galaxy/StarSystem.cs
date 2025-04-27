using EliteJournalReader;
using EliteJournalReader.Events;

namespace ODEliteTracker.Models.Galaxy
{
    public class StarSystem
    {
        public StarSystem(LocationEvent.LocationEventArgs location)
        {
            Name = location.StarSystem;
            Address = location.SystemAddress;
            Position = new(location.StarPos);
            ControllingPower = location.ControllingPower;
            PowerState = location.PowerplayState;
            ControllingFaction = location.SystemFaction?.Name;
            SystemAllegiance = location.SystemAllegiance;
        }

        public StarSystem(FSDJumpEvent.FSDJumpEventArgs fsdJump)
        {
            Name = fsdJump.StarSystem;
            Address = fsdJump.SystemAddress;
            Position = new(fsdJump.StarPos);
            ControllingPower = fsdJump.ControllingPower;
            PowerState = fsdJump.PowerplayState;
            ControllingFaction = fsdJump.SystemFaction?.Name;
            SystemAllegiance = fsdJump.SystemAllegiance;
        }

        public string Name { get; set; }
        public long Address { get; set; }
        public Position Position { get; set; }
        public string? ControllingPower { get; set; }
        public PowerplayState PowerState { get; set; }
        public string? ControllingFaction { get; set; }
        public string? SystemAllegiance { get; set; }
    }
}
