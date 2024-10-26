using Identity.API.Models;
using Newtonsoft.Json;
using System.Net;

namespace Identity.API.Applications.Networks
{
    public static class GeoLocation
    {
        public static async Task<Location> GetLocationAsync(string ipAddress)
        {
            var result = (await new HttpClient().GetStringAsync("http://ip-api.com/json/"+ipAddress))
                .Replace("\\r\\n", "").Replace("\\n", "").Trim();

            Location location = new Location();
            location = JsonConvert.DeserializeObject<Location>(result);

            if (location == null )
            {
                location = new Location();
            }
            return location;
        }
    }
}
