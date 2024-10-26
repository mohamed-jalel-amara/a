using System.ComponentModel.DataAnnotations;

namespace Transaction.API.Models.Requests
{
    public class TransactionRequestReq
    {
        public To To { get; set; }
        public AmountValue Value { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Description { get; set; }
    }
}
