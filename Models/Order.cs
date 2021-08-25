using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TomasosASP.Models
{
    public class Order
    {
        public Order()
        {
            Items = new HashSet<OrderItem>();
        }

        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int TotalAmount { get; set; }
        public bool Delivered { get; set; }

        public string CustomerId { get; set; }
        public User Customer { get; set; }
        public ICollection<OrderItem> Items { get; set; }
    }
}
