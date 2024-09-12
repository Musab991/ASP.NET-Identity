using System.ComponentModel.DataAnnotations;

namespace ASP.NET_Identity.Models
{
    public class UserRoleViewModel
    {

        public string UserId { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public bool IsSelected { get; set; }



    }

}
