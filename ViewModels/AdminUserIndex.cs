using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomasosASP.Models;

namespace TomasosASP.ViewModels
{
    public class AdminUserIndex
    {
        public ApplicationUser User { get; set; }

        public string Role { get; set; }
    }
}
