

using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Account.Access.API.Models.Responses
{
    public class AccountAccessResponse
    {
        [JsonProperty(PropertyName = "ID")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "SHORT_NAME")]
        public string Short_name { get; set; }
        [JsonProperty(PropertyName = "DESCRIPTION")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "V_IS_PUBLIC")]
        [System.Text.Json.Serialization.JsonIgnore]
        public int V_is_public { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public bool Is_Public
        {
            get { return V_is_public == 1 ? true : false; }
            set { V_is_public = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "ALIAS")]
        [System.Text.Json.Serialization.JsonIgnore]
        public string Alias { get; set; }
        [JsonProperty(PropertyName = "V_HIDE_METADATA_IF_ALIAS_USED")]
        [System.Text.Json.Serialization.JsonIgnore]
        public int V_Hide_metadata_if_alias_used { get; set; }
        
        public bool Hide_metadata_if_alias_used
        {
            get { return V_Hide_metadata_if_alias_used == 1 ? true : false; }
            set { V_Hide_metadata_if_alias_used = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_ADD_MORE_INFO")]
        [System.Text.Json.Serialization.JsonIgnore]
        public int V_Can_add_more_info { get; set; }
        public bool Can_add_more_info
        {
            get { return V_Can_add_more_info == 1 ? true : false; }
            set { V_Can_add_more_info = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_BK_ACC_BALANCE")]
        [System.Text.Json.Serialization.JsonIgnore]
        public int V_Can_see_bk_acc_balance { get; set; }
        public bool Can_see_bank_account_balance
        {
            get { return V_Can_see_bk_acc_balance == 1 ? true : false; }
            set { V_Can_see_bk_acc_balance = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_BK_ACC_BK_NAME")]
        [System.Text.Json.Serialization.JsonIgnore]
        public int V_Can_see_bk_acc_bk_name { get; set; }
        public bool Can_see_bank_account_bank_name
        {
            get { return V_Can_see_bk_acc_bk_name == 1 ? true : false; }
            set { V_Can_see_bk_acc_bk_name = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_BK_ACC_CURRENCY")]
        [System.Text.Json.Serialization.JsonIgnore]
        public int V_Can_see_bk_acc_currency { get; set; }
        public bool Can_see_bank_account_currency
        {
            get { return V_Can_see_bk_acc_currency == 1 ? true : false; }
            set { V_Can_see_bk_acc_currency = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_BK_ACC_IBAN")]
        [System.Text.Json.Serialization.JsonIgnore]
        public int V_Can_see_bk_acc_iban { get; set; }
        public bool Can_see_bank_account_iban
        {
            get { return V_Can_see_bk_acc_iban == 1 ? true : false; }
            set { V_Can_see_bk_acc_iban = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_BK_ACC_LABEL")]
        [System.Text.Json.Serialization.JsonIgnore]
        public int V_Can_see_bk_acc_label { get; set; }
        public bool Can_see_bank_account_label
        {
            get { return V_Can_see_bk_acc_label == 1 ? true : false; }
            set { V_Can_see_bk_acc_label = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_BK_ACC_NUMBER")]
        [System.Text.Json.Serialization.JsonIgnore]
        public int V_Can_see_bk_acc_number { get; set; }
        public bool Can_see_bank_account_number
        {
            get { return V_Can_see_bk_acc_number == 1 ? true : false; }
            set { V_Can_see_bk_acc_number = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_BK_ACC_OWNERS")]
        [System.Text.Json.Serialization.JsonIgnore]
        public int V_Can_see_bk_acc_owners { get; set; }
        public bool Can_see_bank_account_owners
        {
            get { return V_Can_see_bk_acc_owners == 1 ? true : false; }
            set { V_Can_see_bk_acc_owners = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_BK_ACC_SWIFT_BIC")]
        [System.Text.Json.Serialization.JsonIgnore]
        public int V_Can_see_bk_acc_swift_bic { get; set; }
        public bool Can_see_bank_account_swift_bic
        {
            get { return V_Can_see_bk_acc_swift_bic == 1 ? true : false; }
            set { V_Can_see_bk_acc_swift_bic = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_O_ACC_BK_NAME")]
        [System.Text.Json.Serialization.JsonIgnore]
        public int V_Can_see_o_acc_bk_name { get; set; }
        public bool Can_see_other_account_bank_name
        {
            get { return V_Can_see_o_acc_bk_name == 1 ? true : false; }
            set { V_Can_see_o_acc_bk_name = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_O_ACC_IBAN")]
        [System.Text.Json.Serialization.JsonIgnore]
        public int V_Can_see_o_acc_iban { get; set; }
        public bool Can_see_other_account_iban
        {
            get { return V_Can_see_o_acc_iban == 1 ? true : false; }
            set { V_Can_see_o_acc_iban = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_O_ACC_NUMBER")]
        [System.Text.Json.Serialization.JsonIgnore]
        public int V_Can_see_o_acc_number { get; set; }
        public bool Can_see_other_account_number
        {
            get { return V_Can_see_o_acc_number == 1 ? true : false; }
            set { V_Can_see_o_acc_number = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_TRANS_AMOUNT")]
        [System.Text.Json.Serialization.JsonIgnore]
        public int V_Can_see_trans_amount { get; set; }
        public bool Can_see_transaction_amount
        {
            get { return V_Can_see_trans_amount == 1 ? true : false; }
            set { V_Can_see_trans_amount = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_TRANS_BALANCE")]
        [System.Text.Json.Serialization.JsonIgnore]
        public int V_Can_see_trans_balance { get; set; }
        public bool Can_see_transaction_balance
        {
            get { return V_Can_see_trans_balance == 1 ? true : false; }
            set { V_Can_see_trans_balance = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_TRANS_CURRENCY")]
        [System.Text.Json.Serialization.JsonIgnore]
        public int V_Can_see_trans_currency { get; set; }
        public bool Can_see_transaction_currency
        {
            get { return V_Can_see_trans_currency == 1 ? true : false; }
            set { V_Can_see_trans_currency = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_TRANS_FIN_DATE")]
        [System.Text.Json.Serialization.JsonIgnore]
        public int V_Can_see_trans_fin_date { get; set; }
        public bool Can_see_transaction_finish_date
        {
            get { return V_Can_see_trans_fin_date == 1 ? true : false; }
            set { V_Can_see_trans_fin_date = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_TRANS_O_BK_ACC")]
        [System.Text.Json.Serialization.JsonIgnore]
        public int V_Can_see_trans_o_bk_account { get; set; }
        public bool Can_see_transaction_other_bank_account
        {
            get { return V_Can_see_trans_o_bk_account == 1 ? true : false; }
            set { V_Can_see_trans_o_bk_account = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_SEE_TRANS_T_BK_ACC")]
        [System.Text.Json.Serialization.JsonIgnore]
        public int V_Can_see_trans_t_bk_account { get; set; }
        public bool Can_see_transaction_this_bank_account
        {
            get { return V_Can_see_trans_t_bk_account == 1 ? true : false; }
            set { V_Can_see_trans_t_bk_account = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "V_CAN_ADD_TRANS_REQ_TO_A_ACC")]
        [System.Text.Json.Serialization.JsonIgnore]
        public int V_Can_add_trans_req_to_a_account { get; set; }
        public bool Can_add_transaction_request_to_any_account
        {
            get { return V_Can_add_trans_req_to_a_account == 1 ? true : false; }
            set { V_Can_add_trans_req_to_a_account = value == true ? 1 : 0; }
        }
        [JsonProperty(PropertyName = "ACCOUNT_ID")]
        [System.Text.Json.Serialization.JsonIgnore]
        public string Account_id { get; set; }
        [JsonProperty(PropertyName = "BANK_ID")]
        [System.Text.Json.Serialization.JsonIgnore]
        public string Bank_id { get; set; }
        [JsonProperty(PropertyName = "OWNER_ID")]
        [System.Text.Json.Serialization.JsonIgnore]
        public string Owner_id { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public int Code { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string ErrorMessage { get; set; }
    }
}
