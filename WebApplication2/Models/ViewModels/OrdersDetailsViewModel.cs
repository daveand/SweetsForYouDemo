using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models.ViewModels
{
    public class OrdersDetailsViewModel
    {
        public OrderModel Order { get; set; }
        public ProductModel Product { get; set; }
        public List<OrderDetailModel> OrderDetails { get; set; }
        public OrderDetailModel DetailsList { get; set; }
        public CustomerModel Customer { get; set; }


    }
}
