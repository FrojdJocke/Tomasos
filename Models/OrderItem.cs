using System;
using System.Collections.Generic;

namespace TomasosASP.Models
{
    public partial class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount  => Quantity * Amount;

        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
