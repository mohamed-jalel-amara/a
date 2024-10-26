using Identity.API.Applications.Models.Entities;

namespace Identity.API.Applications.Models.Responses
{
    public class Entitlements
    {
        public IList<EntitlementResponse> list { get; set; } = new List<EntitlementResponse>();
    }
}
