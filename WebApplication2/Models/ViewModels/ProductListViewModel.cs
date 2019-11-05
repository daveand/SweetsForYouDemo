using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Helpers;

namespace WebApplication2.Models.ViewModels
{
    public class ProductListViewModel
    {
        public PaginatedList<ProductModel> Products { get; set; }

    }
}
