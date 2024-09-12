using System.ComponentModel.DataAnnotations;

namespace ASP.NET_Identity.Models
{
    public class CreateRoleViewModel
    {
        [Required]
        [Display(Name ="Role")]
        public string RoleName { get; set; } = null!;
        public string ? Description { get; set; } = string.Empty;

    }
}
