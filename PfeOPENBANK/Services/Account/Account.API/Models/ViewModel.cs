using Newtonsoft.Json;

namespace Account.API.Models
{
    public class ViewModel
    {

        public int Id { get; set; }
        public string Short_name { get; set; }
        public string Description { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool isPublic { get; set; }
        public bool Is_Public
        {
            get { return isPublic; }
        }
        public string Alias { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool hideMetadataIfAliasUsed { get; set; }

        public bool Hide_metadata_if_alias_used
        {
            get { return hideMetadataIfAliasUsed; }
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool canAddMoreInfo { get; set; }
        public bool Can_add_more_info
        {
            get { return canAddMoreInfo; }
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool canSeeBankAccountBalance { get; set; }
        public bool Can_see_bank_account_balance
        {
            get { return canSeeBankAccountBalance; }
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool canSeeBankAccountBankName { get; set; }
        public bool Can_see_bank_account_bank_name
        {
            get { return canSeeBankAccountBankName; }
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool canSeeBankAccountCurrency { get; set; }
        public bool Can_see_bank_account_currency
        {
            get { return canSeeBankAccountCurrency; }
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool canSeeBankAccountIban { get; set; }
        public bool Can_see_bank_account_iban
        {
            get { return canSeeBankAccountIban; }
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool canSeeBankAccountLabel { get; set; }
        public bool Can_see_bank_account_label
        {
            get { return canSeeBankAccountLabel; }
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool canSeeBankAccountNumber { get; set; }
        public bool Can_see_bank_account_number
        {
            get { return canSeeBankAccountNumber; }
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool canSeeBankAccountOwners { get; set; }
        public bool Can_see_bank_account_owners
        {
            get { return canSeeBankAccountOwners; }
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool canSeeBankAccountSwiftBic { get; set; }
        public bool Can_see_bank_account_swift_bic
        {
            get { return canSeeBankAccountSwiftBic; }
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool canSeeOtherAccountBankName { get; set; }
        public bool Can_see_other_account_bank_name
        {
            get { return canSeeOtherAccountBankName; }
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool canSeeOtherAccountIban { get; set; }
        public bool Can_see_other_account_iban
        {
            get { return canSeeOtherAccountIban; }
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool canSeeOtherAccountNumber { get; set; }
        public bool Can_see_other_account_number
        {
            get { return canSeeOtherAccountNumber; }
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool canSeeTransactionAmount { get; set; }
        public bool Can_see_transaction_amount
        {
            get { return canSeeTransactionAmount; }
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool canSeeTransactionBalance { get; set; }
        public bool Can_see_transaction_balance
        {
            get { return canSeeTransactionBalance; }
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool canSeeTransactionCurrency { get; set; }
        public bool Can_see_transaction_currency
        {
            get { return canSeeTransactionCurrency; }
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool canSeeTransactionFinishDate { get; set; }
        public bool Can_see_transaction_finish_date
        {
            get { return canSeeTransactionFinishDate; }
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool canSeeTransactionOtherBankAccount { get; set; }
        public bool Can_see_transaction_other_bank_account
        {
            get { return canSeeTransactionOtherBankAccount; }
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool canSeeTransactionThisBankAccount { get; set; }
        public bool Can_see_transaction_this_bank_account
        {
            get { return canSeeTransactionThisBankAccount; }
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool canAddTransReqToAnyAccount { get; set; }
        public bool Can_add_transaction_request_to_any_account
        {
            get { return canAddTransReqToAnyAccount; }
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public string Account_id { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string Bank_id { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string Owner_id { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public int Code { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string ErrorMessage { get; set; }
    }
}
