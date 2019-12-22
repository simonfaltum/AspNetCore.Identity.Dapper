namespace AspNetCore.Identity.Dapper
{
    public class DBProviderOptions
    {
        public string DbSchema { get; set; } = "[dbo]";

        public string ConnectionString { get; set; }

    }
}
