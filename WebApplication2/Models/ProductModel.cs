using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public decimal Margin { get; set; }
        public string ImageFile { get; set; }
        public string ImageName { get; set; }
        public string ImageDescription { get; set; }
        public CategoryModel Category { get; set; }
        [NotMapped]
        public int Quantity { get; set; }
    }
}
