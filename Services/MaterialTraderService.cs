using Newtonsoft.Json;
using ODEliteTracker.Models.Galaxy;
using System.IO;
using System.Reflection;

namespace ODEliteTracker.Services
{
    public enum TraderType
    {
        Encoded,
        EncodedorRaw,
        Manufactured,
        ManufacturedorRaw,
        Raw,
    }

    public record MaterialTrader(string SystemName, string StationName, Position Position, double DistanceToArrival, string Economy, TraderType TraderType);

    public sealed class MaterialTraderService
    {
        private List<MaterialTrader> _traders = [];

        public Tuple<IEnumerable<MaterialTrader>, IEnumerable<MaterialTrader>, IEnumerable<MaterialTrader>> GetNearestTraders(Position currentPos)
        {
            if (_traders.Count == 0)
            {
                PopulateTraders();
            }

            var manufacturedTraders = _traders.Where(x => x.TraderType == TraderType.Manufactured)
                                              .OrderBy(x => x.Position.DistanceFrom(currentPos))
                                              .Take(5);
            var encodedTraders = _traders.Where(x => x.TraderType == TraderType.Encoded)
                                         .OrderBy(x => x.Position.DistanceFrom(currentPos))
                                         .Take(5);
            var rawTraders = _traders.Where(x => x.TraderType == TraderType.Raw)
                                     .OrderBy(x => x.Position.DistanceFrom(currentPos))
                                     .Take(5);

            return Tuple.Create(manufacturedTraders, encodedTraders, rawTraders);
        }

        public void PopulateTraders()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var names = assembly.GetManifestResourceNames();
            var resourceName = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith("MaterialTraders.jsonl"));

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            using Stream stream = assembly.GetManifestResourceStream(resourceName);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8604 // Possible null reference argument.
            using StreamReader reader = new(stream);
#pragma warning restore CS8604 // Possible null reference argument.
            using var jsonReader = new JsonTextReader(reader);
            jsonReader.SupportMultipleContent = true;
            var serializer = JsonSerializer.Create();

            while (jsonReader.Read())
            {
                var person = serializer.Deserialize<MaterialTrader>(jsonReader);
                if (person != null) 
                    _traders.Add(person);
            }
        }
    }
}
