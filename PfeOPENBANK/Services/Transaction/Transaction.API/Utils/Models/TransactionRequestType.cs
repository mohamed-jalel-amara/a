namespace Transaction.API.Utils.Models
{
    public static class TransactionRequestType
    {
        public static IList<string> Types { get; set; } = new List<string> { "SANDBOX_TAN" };
    }
}
