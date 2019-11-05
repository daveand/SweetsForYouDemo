using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class OrderStateModel
    {
        public int Id { get; set; }
        public bool OrderSent { get; set; }
        public bool OrderDelivered { get; set; }
        public string DateDelivered { get; set; }
        public int OrderId { get; set; }
        public OrderModel Order { get; set; }
    }
}
