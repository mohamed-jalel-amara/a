namespace Account.Access.API.Models
{
    public class Allowed_actions
    {
        public bool Can_add_more_info { get; set; }
        public bool Can_see_bank_account_balance { get; set; }
        public bool Can_see_bank_account_bank_name { get; set; }
        public bool Can_see_bank_account_currency { get; set; }
        public bool Can_see_bank_account_iban { get; set; }
        public bool Can_see_bank_account_label { get; set; }
        public bool Can_see_bank_account_number { get; set; }
        public bool Can_see_bank_account_owners { get; set; }
        public bool Can_see_bank_account_swift_bic { get; set; }
        public bool Can_see_other_account_bank_name { get; set; }
        public bool Can_see_other_account_iban { get; set; }
        public bool Can_see_other_account_number { get; set; }
        public bool Can_see_transaction_amount { get; set; }
        public bool Can_see_transaction_balance { get; set; }
        public bool Can_see_transaction_currency { get; set; }
        public bool Can_see_transaction_finish_date { get; set; }
        public bool Can_see_transaction_other_bank_account { get; set; }
        public bool Can_see_transaction_this_bank_account { get; set; }
        public bool Can_add_transaction_request_to_any_account { get; set; }
    }
}
