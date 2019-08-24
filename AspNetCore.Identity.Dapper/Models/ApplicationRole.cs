using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.Models
{
    public class ApplicationRole : IdentityRole
    {
        internal List<Claim> Claims { get; set; }
    }
}
