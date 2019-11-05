using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models.ViewModels
{
    public class CustomerInfoModel
    {
        public CustomerModel CustomerModel { get; set; }
        public IdentityUser User { get; set; }
        public List<OrderModel> Orders { get; set; }

        public int OrderTotal { get; set; }
        public int TotalMargin { get; set; }
        public List<OrderSummaryViewModel> OrderSummary { get; set; }

        public bool NotEmpty
        {
            get { return (Orders.Any(m => m.OrderState.OrderDelivered != true)); }
        }

    }
}
