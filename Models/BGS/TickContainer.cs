using ODEliteTracker.Database.DTOs;

namespace ODEliteTracker.Models.BGS
{
    public class TickData(string iD, DateTime from, DateTime to)
    {
        public string ID { get; } = iD;
        public DateTime From { get; } = from;
        public DateTime To { get; } = to;

        public bool TimeWithinTick(DateTime time)
        {
            return time >= From && time < To;
        }

        public bool TimeWithinTick(IEnumerable<DateTime> times)
        {
            foreach (var time in times)
            {
                if (time >= From && time < To)
                    return true;
            }

            return false;
        }
    }

    public sealed class TickContainer(List<BGSTickData> tickData)
    {
        private List<BGSTickData> tickData = [.. tickData.OrderBy(x => x.Time)];

        public List<BGSTickData> TickData => tickData;

        public void UpdateTickData(List<BGSTickData> tickData)
        {
            this.tickData = [.. tickData.OrderBy(x => x.Time)];
        }

        public BGSTickData? GetTick(DateTime value)
        {
            if (tickData == null || tickData.Count == 0)
            {
                return null;
            }

            var count = tickData.Count;

            for (var i = 0; i < count - 1; i++)
            {
                var starDate = tickData[i].Time;
                var endDate = tickData[i + 1].Time;

                if (value >= starDate && value < endDate)
                {
                    return tickData[i];
                }
            }

            return tickData[count - 1];
        }

        public TickData GetTickFromTo(string id)
        {
            var data = TickData.FirstOrDefault(x => string.Equals(id, x.Id))
                ?? new BGSTickData("Not Found", DateTime.MinValue, DateTime.MinValue);

            return GetTickFromTo(data);
        }

        public TickData GetTickFromTo(BGSTickData data)
        {
            var idx = tickData.IndexOf(data);

            if (idx >= 0)
            {
                var from = data.Time;
                var to = idx < tickData.Count - 1 ? tickData[idx + 1].Time : DateTime.MaxValue;

                return new(data.Id, from, to);
            }

            return new("Not Found", DateTime.MinValue, DateTime.MinValue);
        }
    }
}
