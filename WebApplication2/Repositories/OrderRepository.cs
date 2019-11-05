using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication2.Data;
using WebApplication2.Interfaces;
using WebApplication2.Models;
using WebApplication2.Models.ViewModels;

namespace WebApplication2.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly ShoppingCart _shoppingCart;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderRepository(ApplicationDbContext appDbContext, ShoppingCart shoppingCart, IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext;
            _shoppingCart = shoppingCart;
            _httpContextAccessor = httpContextAccessor;
        }

        public void CreateOrder(OrderModel order)
        {
            order.OrderPlaced = DateTime.Now;
            order.OrderTotal = _shoppingCart.GetShoppingCartTotal();
            order.UserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            _appDbContext.OrderModel.Add(order);

            var orderState = new OrderStateModel { OrderId = order.Id, OrderSent = false };
            _appDbContext.OrderStateModel.Add(orderState);

            var shoppingCartItems = _shoppingCart.ShoppingCartItems;

            foreach (var shoppingCartItem in shoppingCartItems)
            {
                var orderDetail = new OrderDetailModel()
                {
                    Quantity = shoppingCartItem.Quantity,
                    ProductId = shoppingCartItem.Product.Id,
                    ProductTitle = shoppingCartItem.Product.Title,
                    OrderId = order.Id,
                    Price = shoppingCartItem.Product.Price

                };

                _appDbContext.OrderDetailModel.Add(orderDetail);
            }

            _appDbContext.SaveChanges();
        }

    }
}
