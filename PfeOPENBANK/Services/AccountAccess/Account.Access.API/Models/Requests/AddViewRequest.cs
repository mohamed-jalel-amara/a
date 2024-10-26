using System.ComponentModel.DataAnnotations;

namespace Account.Access.API.Models.Requests
{
    public class AddViewRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Alias { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Description { get; set; }
        public bool Is_public { get; set; }
        public string Which_alias_to_use { get; set; }
        public bool Hide_metadata_if_alias_used { get; set; }
        public IList<string> Allowed_actions { get; set; } = new List<string>();
    }
}
