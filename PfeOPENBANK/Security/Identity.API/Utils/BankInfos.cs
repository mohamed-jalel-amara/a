namespace Identity.API.Utils
{
    public static class BankInfos
    {
        public static string Bank_Id { get; set; } = "gh.29.uk";
        public static IList<string> Roles { get; set; } = new List<string>() { "CanGetAnyUser", 
            "CanQueryOtherUser", "CanCreateAccount", "CanGrantAccessToViews", "CanCreateEntitlementAtOneBank",
            "CanCreateBranch", "CanCreateAnyTransactionRequest", "CanCreateBank" };
    }
}
