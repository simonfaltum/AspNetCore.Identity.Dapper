namespace AspNetCore.Identity.Dapper.Stores
{
    internal class RoleClaim
    {
        public string Id { get; set; }
        public string RoleId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
