using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ODEliteTracker.Database.DTOs
{
    [PrimaryKey(nameof(Id))]
    public class BGSTickData(string id, DateTime time, DateTime updated_At)
    {
        [JsonProperty("_id")]
        public string Id { get; set; } = id;
        public DateTime Time { get; set; } = time;
        public DateTime Updated_At { get; set; } = updated_At;
        public bool ManualTick { get; set; } = false;

        public override bool Equals(object? obj)
        {
            if (obj is BGSTickData otherData)
            {
                return string.Equals(otherData.Id, Id);
            }
            return false;
        }

        public override int GetHashCode()
        {
            if (Id is null)
            {
                return base.GetHashCode();
            }

            return Id.GetHashCode();
        }
    }
}
