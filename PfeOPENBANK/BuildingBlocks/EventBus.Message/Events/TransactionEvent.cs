namespace EventBus.Message.Events
{
    public enum TRANS_STATUS
    {
        SUCCESS, FAILED, WAITING, CANCELED
    }
    public class TransactionEvent : IntegrationBaseEvent
    {
        public Int64 TRANSID { get; set; }
        public string AUTHOR { get; set; }
        public string DESCRIPTION { get; set; }
        public decimal AMOUNT { get; set; }
        public string STATUS { get; set; }
        public string TYPE { get; set; }
        public Int64 CREDITEDACC { get; set; }
        public Int64 DEBITEDACC { get; set; }
        public DateTime CREATEDAT { get; set; }
        public DateTime UPDATEDAT { get; set; }
    }
}
