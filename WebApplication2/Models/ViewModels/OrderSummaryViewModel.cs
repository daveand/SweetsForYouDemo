using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models.ViewModels
{
    public class OrderSummaryViewModel
    {
        public string ProductTitle { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public int SubTotal { get; set; }
        public int MarginPercent { get; set; }
        public decimal Margin { get; set; }
    }
}
