using ODEliteTracker.Database.DTOs;
using System.Net.Http;
using System.Net.Http.Json;

namespace ODEliteTracker.Services
{
    public sealed class EliteBGSApiService(HttpClient client)
    {
        private readonly HttpClient client = client;

        public async Task<List<BGSTickData>> GetTicks(long min = 0, long max = 0)
        {
            if(max == 0)
                max = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var content = await client.GetFromJsonAsync<List<BGSTickData>>($"ticks?timeMin={min}&timeMax={max}").ConfigureAwait(true);

            if (content == null)
                return [new BGSTickData("initial", new(2014, 12, 16), new(2014, 12, 16), true)];

            return content;
        }
    }
}
