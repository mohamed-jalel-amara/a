namespace Account.API.Models.Responses
{
    public class AccountMinimal
    {
        public string Bank_id { get; set; }
        public string Account_id { get; set; }
        public string View_id { get; set; } = "owner";
    }
}
