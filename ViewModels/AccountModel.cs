using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomasosASP.Models;

namespace TomasosASP.ViewModels
{
    public class AccountModel
    {
        public User Customer { get; set; }
        public string Password { get; set; }
        public int itemsInCart { get; set; }
        public bool Unique { get; set; }
    }
}
