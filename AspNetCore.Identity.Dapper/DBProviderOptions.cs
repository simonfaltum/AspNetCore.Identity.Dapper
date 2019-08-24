using System;
using System.Collections.Generic;
using System.Data.Common;
using Dapper;

namespace AspNetCore.Identity.Dapper
{
    public class DBProviderOptions
    {
        public string DbSchema { get; set; } = "[dbo]";

        public string ConnectionString { get; set; }

    }
}
