
using Google.Protobuf.WellKnownTypes;

namespace Branch.API.Models
{
    public class BranchModel
    {

        #region Fields
        public string ID { get; set; }
        public string BRANCH_TYPE { get; set; }
        public string BANK_ID { get; set; }
        public string NAME { get; set; }
        public string LINE_1 { get; set; }
        public string LINE_2 { get; set; }
        public string LINE_3 { get; set; }
        public string CITY { get; set; }
        public string STATE { get; set; }
        public string COUNTRY { get; set; }
        public int POSTCODE { get; set; }
        public string COUNTRY_CODE { get; set; }
        public double LATITUDE { get; set; }
        public double LONGITUDE { get; set; }
        public string LICENCE_ID { get; set; }
        public string LICENCE_NAME { get; set; }
        public string ROUTING_SCHEME { get; set; }
        public string ROUTING_ADDRESS { get; set; }
        public bool ACCESSIBLE_VALUE
        {
            get
            {
                return IS_ACCESSIBLE == 1 ? true : false;
            }
            set
            {
                IS_ACCESSIBLE = value ? 1 : 0;
            }
        }
        public int IS_ACCESSIBLE { get; set; }
        public bool HAS_LOBBY 
        {  get
            {
                return LOBBY == 1 ? true : false;
            } 
            set {
                LOBBY = value? 1 : 0;
            } 
        }
        public int LOBBY { get; set; }
        public bool HAS_DRIVE_UP
        {
            get
            {
                return DRIVE_UP == 1 ? true : false;
            }
            set
            {
                DRIVE_UP = value ? 1 : 0;
            }
        }
        public int DRIVE_UP { get; set; }
        public DateTime OPENING_TIME { get; set; }
        public DateTime CLOSING_TIME { get; set; }
        public bool ON_MONDAY
        {
            get
            {
                return MONDAY == 1 ? true : false;
            }
            set
            {
                MONDAY = value ? 1 : 0;
            }
        }
        public int MONDAY { get; set; }
        public bool ON_TUESDAY
        {
            get
            {
                return TUESDAY == 1 ? true : false;
            }
            set
            {
                TUESDAY = value ? 1 : 0;
            }
        }
        public int TUESDAY { get; set; }
        public bool ON_WEDNESDAY
        {
            get
            {
                return WEDNESDAY == 1 ? true : false;
            }
            set
            {
                WEDNESDAY = value ? 1 : 0;
            }
        }
        public int WEDNESDAY { get; set; }
        public bool ON_THURSDAY
        {
            get
            {
                return THURSDAY == 1 ? true : false;
            }
            set
            {
                THURSDAY = value ? 1 : 0;
            }
        }
        public int THURSDAY { get; set; }
        public bool ON_FRIDAY
        {
            get
            {
                return FRIDAY == 1 ? true : false;
            }
            set
            {
                FRIDAY = value ? 1 : 0;
            }
        }
        public int FRIDAY { get; set; }
        public bool ON_SATURDAY
        {
            get
            {
                return SATURDAY == 1 ? true : false;
            }
            set
            {
                SATURDAY = value ? 1 : 0;
            }
        }
        public int SATURDAY { get; set; }
        public bool ON_SUNDAY
        {
            get
            {
                return SUNDAY == 1 ? true : false;
            }
            set
            {
                SUNDAY = value ? 1 : 0;
            }
        }
        public int SUNDAY { get; set; }
        public string ACCESSIBLE_FEATURES { get; set; }
        public string MORE_INFO { get; set; }
        public string PHONE_NUMBER { get; set; }
        public DateTime CREATEDAT { get; set; }
        public DateTime UPDATEDAT { get; set; }
        public Int64 BRANCH_NUM { get; set; }
        #endregion
    }
}
