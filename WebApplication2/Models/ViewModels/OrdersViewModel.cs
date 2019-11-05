using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models.ViewModels
{
    public class OrdersViewModel
    {
        public List<OrderModel> Order { get; set; }
        public List<CustomerModel> Customer { get; set; }

    }
}
