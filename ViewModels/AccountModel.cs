using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomasosASP.Models;

namespace TomasosASP.ViewModels
{
    public class AccountModel
    {
        public Kund Customer { get; set; }
        public int itemsInCart { get; set; }
    }
}
