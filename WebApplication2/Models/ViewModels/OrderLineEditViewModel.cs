using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models.ViewModels
{
    public class OrderLineEditViewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
