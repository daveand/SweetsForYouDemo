using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<WebApplication2.Models.CustomerModel> CustomerModel { get; set; }
        public DbSet<WebApplication2.Models.ProductModel> ProductModel { get; set; }
        public DbSet<WebApplication2.Models.CategoryModel> CategoryModel { get; set; }
        public DbSet<WebApplication2.Models.ShoppingCartItemModel> ShoppingCartItemModel { get; set; }
        public DbSet<WebApplication2.Models.OrderModel> OrderModel { get; set; }
        public DbSet<WebApplication2.Models.OrderDetailModel> OrderDetailModel { get; set; }
        public DbSet<WebApplication2.Models.OrderStateModel> OrderStateModel { get; set; }




    }
}
