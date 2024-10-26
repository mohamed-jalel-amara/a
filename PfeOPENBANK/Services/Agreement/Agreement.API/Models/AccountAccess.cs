namespace Agreement.API.Models
{
    public enum ACCESS_STATUS
    {
        GRANTED,EXPIRED,REVOKE,WAITING
    }
    public class AccountAccess
    {
        public int ID { get; set; }
        public long ACCNUMBER { get; set; }
        public string PROVIDERID { get; set; }
        public int DURATION { get; set; }
        public string STATUS { get; set; }
        public DateTime CREATEDAT { get; set; }
        public DateTime UPDATEDAT { get; set; }
        public string CUSTOMERID { get; set; }
    }
}
