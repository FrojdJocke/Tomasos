using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomasosASP.Models;

namespace TomasosASP.ViewModels
{
    public class AdminOrderView
    {
        public List<Order> Orders { get; set; }
        public List<User> Customers { get; set; }
        public List<OrderItem> BestallningMatratts { get; set; }
    }
}
