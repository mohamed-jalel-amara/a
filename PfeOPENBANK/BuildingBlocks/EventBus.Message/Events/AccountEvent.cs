namespace EventBus.Message.Events
{
    public class AccountEvent : IntegrationBaseEvent
    {
        public Int64 ACCNUMBER { get; set; }
        public string OWNERFIRSTNAME { get; set; }
        public string OWNERLASTNAME { get; set; }
        public string OWNEREMAIL { get; set; }
        public string OWNERPHONE { get; set; }
        public string OWNERADDRESS { get; set; }
        public int OWNERPOSTCODE { get; set; }
        public string BANKNAME { get; set; }
        public string BANKCODE { get; set; }
        public int BRANCHCODE { get; set; }
        public int BANKDETAILSKEY { get; set; }
        public string IBAN { get; set; }
        public double BALANCE { get; set; }
        public DateTime CREATEDAT { get; set; }
        public DateTime UPDATEDAT { get; set; }
        public string STATUS { get; set; }
        public Int64 TRANSACTIONEVENTID { get; set; }
    }
}
