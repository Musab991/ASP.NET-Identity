using Microsoft.AspNetCore.Identity;

namespace ASP.NET_Identity.Models
{
    public class ApplicationRole:IdentityRole
    {
        //Add custom properties here..
        public string Description { get; set; } = null!;

    }
}
