namespace Account.Access.API.Models
{
    public class AccountModel
    {
        public long ACCNUMBER { get; set; }
        public string BANKNAME { get; set; }
        public string BANKCODE { get; set; }
        public int BRANCHCODE { get; set; }
        public string IBAN { get; set; }
        public double BALANCE { get; set; }
    }
}
