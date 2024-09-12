﻿using System.ComponentModel.DataAnnotations;

namespace ASP.NET_Identity.Models
{
    public class EditRoleViewModel
    {
        [Required]
        public string Id { get; set; }
        [Required(ErrorMessage = "Role Name is Required")]
        public string RoleName { get; set; } = null!;

        public string ? Description {  get; set; }

        public List<string>?Users { get; set; }


    }
}
