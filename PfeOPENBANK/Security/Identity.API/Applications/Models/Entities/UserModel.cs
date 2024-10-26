using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Identity.API.Applications.Models.Entities
{
    public class UserModel : IdentityUser
    {
        public string? First_name { get; set; }
        public string? Last_name { get; set; }
        public string? Provider_id { get; set; }
        [DataType(DataType.Url)]
        public string? Provider { get; set; }
    }
}
