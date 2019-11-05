using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models.ViewModels
{
    public class SendOrderViewModel
    {
        public List<OrderModel> Orders { get; set; }
        public List<OrderDetailModel> Details { get; set; }

    }
}
