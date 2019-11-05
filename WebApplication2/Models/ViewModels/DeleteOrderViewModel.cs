using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models.ViewModels
{
    public class DeleteOrderViewModel
    {
        public OrderModel Order { get; set; }
        public CustomerModel Customer { get; set; }
    }
}
