using Microsoft.AspNetCore.Identity;

namespace Identity.API.Applications.Models.Entities
{
    public class Entitlement : IdentityRole
    {
        public string Bank_id { get; set; }
    }
}
