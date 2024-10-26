using Newtonsoft.Json;

namespace View.grpc.Models
{
    public class ViewModel
    {
        [JsonProperty(PropertyName = "ID")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "ALIAS")]
        public string Alias { get; set; }
        [JsonProperty(PropertyName = "WHICH_ALIAS_TO_USE")]
        public string WhichAliasToUse { get; set; }
        [JsonProperty(PropertyName = "SHORT_NAME")]
        public string ShortName { get; set; }
        [JsonProperty(PropertyName = "DESCRIPTION")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "V_IS_PUBLIC")]
        public int V_is_public { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public bool IsPublic
        {
            get { return V_is_public == 1 ? true : false; }
            set { V_is_public = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_HIDE_METADATA_IF_ALIAS_USED")]
        public int V_Hide_metadata_if_alias_used { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public bool HideMetadataIfAliasUsed
        {
            get { return V_Hide_metadata_if_alias_used == 1 ? true : false; }
            set { V_Hide_metadata_if_alias_used = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_ADD_MORE_INFO")]
        public int V_Can_add_more_info { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool CanAddMoreInfo
        {
            get { return V_Can_add_more_info == 1 ? true : false; }
            set { V_Can_add_more_info = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_BK_ACC_BALANCE")]
        public int V_Can_see_bk_acc_balance { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool CanSeeBankAccountBalance
        {
            get { return V_Can_see_bk_acc_balance == 1 ? true : false; }
            set { V_Can_see_bk_acc_balance = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_BK_ACC_BK_NAME")]
        public int V_Can_see_bk_acc_bk_name { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool CanSeeBankAccountBankName
        {
            get { return V_Can_see_bk_acc_bk_name == 1 ? true : false; }
            set { V_Can_see_bk_acc_bk_name = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_BK_ACC_CURRENCY")]
        public int V_Can_see_bk_acc_currency { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool CanSeeBankAccountCurrency
        {
            get { return V_Can_see_bk_acc_currency == 1 ? true : false; }
            set { V_Can_see_bk_acc_currency = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_BK_ACC_IBAN")]
        public int V_Can_see_bk_acc_iban { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool CanSeeBankAccountIban
        {
            get { return V_Can_see_bk_acc_iban == 1 ? true : false; }
            set { V_Can_see_bk_acc_iban = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_BK_ACC_LABEL")]
        public int V_Can_see_bk_acc_label { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool CanSeeBankAccountLabel
        {
            get { return V_Can_see_bk_acc_label == 1 ? true : false; }
            set { V_Can_see_bk_acc_label = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_BK_ACC_NUMBER")]
        public int V_Can_see_bk_acc_number { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool CanSeeBankAccountNumber
        {
            get { return V_Can_see_bk_acc_number == 1 ? true : false; }
            set { V_Can_see_bk_acc_number = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_BK_ACC_OWNERS")]
        public int V_Can_see_bk_acc_owners { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool CanSeeBankAccountOwners
        {
            get { return V_Can_see_bk_acc_owners == 1 ? true : false; }
            set { V_Can_see_bk_acc_owners = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_BK_ACC_SWIFT_BIC")]
        public int V_Can_see_bk_acc_swift_bic { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool CanSeeBankAccountSwiftBic
        {
            get { return V_Can_see_bk_acc_swift_bic == 1 ? true : false; }
            set { V_Can_see_bk_acc_swift_bic = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_O_ACC_BK_NAME")]
        public int V_Can_see_o_acc_bk_name { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool CanSeeOtherAccountBankName
        {
            get { return V_Can_see_o_acc_bk_name == 1 ? true : false; }
            set { V_Can_see_o_acc_bk_name = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_O_ACC_IBAN")]
        public int V_Can_see_o_acc_iban { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool CanSeeOtherAccountIban
        {
            get { return V_Can_see_o_acc_iban == 1 ? true : false; }
            set { V_Can_see_o_acc_iban = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_O_ACC_NUMBER")]
        public int V_Can_see_o_acc_number { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool CanSeeOtherAccountNumber
        {
            get { return V_Can_see_o_acc_number == 1 ? true : false; }
            set { V_Can_see_o_acc_number = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_TRANS_AMOUNT")]
        public int V_Can_see_trans_amount { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool CanSeeTransactionAmount
        {
            get { return V_Can_see_trans_amount == 1 ? true : false; }
            set { V_Can_see_trans_amount = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_TRANS_BALANCE")]
        public int V_Can_see_trans_balance { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool CanSeeTransactionBalance
        {
            get { return V_Can_see_trans_balance == 1 ? true : false; }
            set { V_Can_see_trans_balance = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_TRANS_CURRENCY")]
        public int V_Can_see_trans_currency { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool CanSeeTransactionCurrency
        {
            get { return V_Can_see_trans_currency == 1 ? true : false; }
            set { V_Can_see_trans_currency = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_TRANS_FIN_DATE")]
        public int V_Can_see_trans_fin_date { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool CanSeeTransactionFinishDate
        {
            get { return V_Can_see_trans_fin_date == 1 ? true : false; }
            set { V_Can_see_trans_fin_date = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_TRANS_O_BK_ACC")]
        public int V_Can_see_trans_o_bk_account { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool CanSeeTransactionOtherBankAccount
        {
            get { return V_Can_see_trans_o_bk_account == 1 ? true : false; }
            set { V_Can_see_trans_o_bk_account = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_TRANS_T_BK_ACC")]
        public int V_Can_see_trans_t_bk_account { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool CanSeeTransactionThisBankAccount
        {
            get { return V_Can_see_trans_t_bk_account == 1 ? true : false; }
            set { V_Can_see_trans_t_bk_account = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_ADD_TRANS_REQ_TO_A_ACC")]
        public int V_Can_add_trans_req_to_a_account { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool CanAddTransReqToAnyAccount
        {
            get { return V_Can_add_trans_req_to_a_account == 1 ? true : false; }
            set { V_Can_add_trans_req_to_a_account = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "ACCOUNT_ID")]
        public string AccountId { get; set; }
        [JsonProperty(PropertyName = "BANK_ID")]
        public string BankId { get; set; }
        [JsonProperty(PropertyName = "CREATED_AT")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty(PropertyName = "UPDATED_AT")]
        public DateTime UpdatedAt { get; set; }
        [JsonProperty(PropertyName = "OWNER_ID")]
        public string OwnerId { get; set; }
    }
}
