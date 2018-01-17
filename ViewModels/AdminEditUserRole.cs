using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using TomasosASP.Models;

namespace TomasosASP.ViewModels
{
    public class AdminEditUserRole
    {

        [Required]
        public ApplicationUser User { get; set; }

        [DisplayName("Current Role")]
        public string Role { get; set; }

        public List<SelectListItem> Roles { get; set; }

        [Required]
        [DisplayName("New Role")]
        public string SelectedRole { get; set; }

    }
}
