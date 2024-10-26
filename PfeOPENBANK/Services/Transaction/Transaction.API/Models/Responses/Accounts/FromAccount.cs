namespace Transaction.API.Models.Responses.Accounts
{
    public class FromAccount
    {
        public string Id { get; set; }
        public IList<AccountView> Holders { get; set; } = new List<AccountView>();
    }
}
