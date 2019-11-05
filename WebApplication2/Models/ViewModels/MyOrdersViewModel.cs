using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Helpers;

namespace WebApplication2.Models.ViewModels
{
    public class MyOrdersViewModel
    {
        public PaginatedList<OrderModel> PendingOrders { get; set; }
        public PaginatedList<OrderModel> DeliveredOrders { get; set; }
        public List<OrderStateModel> OrderState { get; set; }

        public int TotalSales { get; set; }
        public int TotalMargin { get; set; }
        public int OrderQuantity { get; set; }
        public int Participants { get; set; }

        public bool PendingNotEmpty {
            get { return (PendingOrders.Any(m => m.OrderState.OrderSent == false)); }
        }

    }
}
