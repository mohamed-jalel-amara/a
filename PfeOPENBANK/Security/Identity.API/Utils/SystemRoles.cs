namespace Identity.API.Utils
{
    public static class SystemRoles
    {
        public static IList<string> Roles { get; set; } = new List<string>() { "SUPERADMIN", "ADMIN", "CUSTOMER" };
    }
}
