using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models.ViewModels;

namespace WebApplication2.Models
{
    public class OrdersPdfModel
    {
        public CustomerModel Customer { get; set; }
        public int OrderTotal { get; set; }
        public int TotalMargin { get; set; }

        public List<OrderSummaryViewModel> OrderSummary { get; set; }
        public List<OrderModel> Orders { get; set; }
    }
}
