using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODEliteTracker.Models.BGS
{
    internal sealed class SystemConflictManager
    {
        internal ConflictType conflictType;

        internal void OnDestinationDrop(string destinationType)
        {
            if (destinationType.StartsWith("$Warzone_PointRace") == false)
                return;

            if (destinationType.StartsWith("$Warzone_PointRace_Low"))
            {
                conflictType = ConflictType.LowSpaceCZ;
            }

            if (destinationType.StartsWith("$Warzone_PointRace_Med"))
            {
                conflictType = ConflictType.MediumSpaceCZ;
            }

            conflictType = ConflictType.HighSpaceCZ;
        }
    }
}
