using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class OrderDetailModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductTitle { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public virtual ProductModel Product { get; set; }
        public virtual OrderModel Order { get; set; }

    }
}
