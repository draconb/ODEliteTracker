using ODEliteTracker.ViewModels.ModelViews.BGS;
using System.Text;

namespace ODEliteTracker.Helpers
{
    internal class DiscordPostCreator
    {
        public static bool CreateBGSPost(IEnumerable<BGSTickSystemVM> systems, TickDataVM data)
        {
            systems = systems.Where(x => x.HasData).OrderBy(x => x.Name);

            var builder = new StringBuilder();

            builder.AppendLine($"__**BGS Report - Tick : {DiscordTimeConvertor(data.TickTime)}**__");
            builder.AppendLine();

            foreach (var system in systems)
            {
                builder.AppendLine($"> **{system.NonUpperName}**");

                foreach(var faction in system.Factions)
                {
                    if (faction.HasData() == false)
                        continue;

                    builder.AppendLine(FactionDataString(faction));
                }
            }

            var result = builder.ToString().TrimEnd('\r', '\n');

            return ODMVVM.Helpers.OperatingSystem.SetStringToClipboard(result);
        }

        private static string FactionDataString(FactionVM? data)
        {
            if (data is null)
            {
                return string.Empty;
            }

            StringBuilder builder = new();

            builder.Append(">    ");

            builder.Append($"{data.Name} : ");

            //Inf
            if (data.InfPlus != 0)
            {
                builder.Append($"{data.InfPlus:+#;-#;0} Inf, ");
            }

            //Trade Spend
            if (data.Purchases?.Count > 0)
            {
                builder.Append("Spend ");
                builder.Append($"{data.Purchases.ToString()}");
                builder.Append(", ");
            }
            //Trade sales
            if (data.Sales?.Count > 0)
            {
                builder.Append("Sales ");
                builder.Append($"{data.Sales.ToString()}");
                builder.Append(", ");
            }

            //S&R sales
            if (data.SearchAndRescue?.Total > 0)
            {
                builder.Append($"S&R {data.SearchAndRescue.Total} unit{(data.SearchAndRescue.Total > 1 ? "s, " : ", ")}");
            }
            //Carto
            if (data.CartoDataValue > 0)
            {
                builder.Append($"{data.CartoData} Carto, ");
            }
            //Exo
            //if (data.ExobiologySales > 0)
            //{
            //    builder.Append($"{Helpers.FormatNumber(data.ExobiologySales)} Exo, ");
            //}
            //Bounties
            if (data.Bounties > 0)
            {
                builder.Append($"{data.BountyVouchers} BVs , ");
            }
            //Bonds
            if (data.Bonds > 0)
            {
                builder.Append($"{data.BondVouchers} CBs, ");
            }
            //Failed missions
            if (data.Failed > 0)
            {
                builder.Append($"{data.MissionsFailed}x Failed, ");
            }
            //Murdered
            if (data.TotalMurders > 0)
            {
                if (data.ShipMurders > 0)
                {
                    builder.Append($"{data.ShipMurders}x Ship Murder{(data.ShipMurders > 1 ? "s," : ",")} ");
                }
                if (data.FootMurders > 0)
                {
                    builder.Append($"{data.FootMurders}x Foot Murder{(data.FootMurders > 1 ? "s," : ",")} ");
                }
            }
            //TODO
            //if (data.CZData.ConflictCount != 0)
            //{
            //    if (data.CZData.GroundCountByType(GroundConflictType.Shutdown) != 0)
            //    {
            //        builder.Append($"{data.CZData.GroundCountByType(GroundConflictType.Shutdown)}x Shutdown{(data.CZData.GroundCountByType(GroundConflictType.Shutdown) > 1 ? "s," : ",")} ");
            //    }
            //    if (data.CZData.GroundCountByType(GroundConflictType.LowCZ) != 0)
            //    {
            //        builder.Append($"{data.CZData.GroundCountByType(GroundConflictType.LowCZ)}x LGCZ, ");
            //    }
            //    if (data.CZData.GroundCountByType(GroundConflictType.MediumCZ) != 0)
            //    {
            //        builder.Append($"{data.CZData.GroundCountByType(GroundConflictType.MediumCZ)}x MGCZ, ");
            //    }
            //    if (data.CZData.GroundCountByType(GroundConflictType.HighCZ) != 0)
            //    {
            //        builder.Append($"{data.CZData.GroundCountByType(GroundConflictType.HighCZ)}x HGCZ, ");
            //    }
            //    if (data.CZData.SpaceConflicts.Low != 0)
            //    {
            //        builder.Append($"{data.CZData.SpaceConflicts.Low}x LSCZ, ");
            //    }
            //    if (data.CZData.SpaceConflicts.Medium != 0)
            //    {
            //        builder.Append($"{data.CZData.SpaceConflicts.Medium}x MSCZ, ");
            //    }
            //    if (data.CZData.SpaceConflicts.High != 0)
            //    {
            //        builder.Append($"{data.CZData.SpaceConflicts.High}x HSCZ, ");
            //    }
            //    if (data.CZData.Scenarios != 0)
            //    {
            //        builder.Append($"{data.CZData.Scenarios}x Scenarios, ");
            //    }
            //}

            return builder.ToString().TrimEnd('\r', '\n').TrimEnd(',', ' ');
        }

        public static string DiscordTimeConvertor(DateTime time)
        {
            TimeSpan t = time.ToUniversalTime() - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;
            return $"<t:{secondsSinceEpoch}>";
        }
    }
}
