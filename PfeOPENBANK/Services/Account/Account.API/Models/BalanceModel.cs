using System.ComponentModel.DataAnnotations;

namespace Account.API.Models
{
    public class BalanceModel
    {
        [Required(AllowEmptyStrings = false)]
        public string Currency { get; set; }
        public double Amount { get; set; }
    }
}
