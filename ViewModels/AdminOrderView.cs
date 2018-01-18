using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomasosASP.Models;

namespace TomasosASP.ViewModels
{
    public class AdminOrderView
    {
        public List<Bestallning> Orders { get; set; }
        public List<Kund> Customers { get; set; }
        public List<BestallningMatratt> BestallningMatratts { get; set; }
    }
}
