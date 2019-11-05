using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class ShoppingCartItemModel
    {
        public int Id { get; set; }
        public ProductModel Product { get; set; }
        public int Quantity { get; set; }
        public string ShoppingCartId { get; set; }
    }
}
