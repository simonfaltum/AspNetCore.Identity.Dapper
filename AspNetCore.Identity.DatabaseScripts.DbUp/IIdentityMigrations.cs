namespace AspNetCore.Identity.DatabaseScripts.DbUp
{
    public interface IIdentityMigrations
    {
        bool UpgradeDatabase();
    }
}