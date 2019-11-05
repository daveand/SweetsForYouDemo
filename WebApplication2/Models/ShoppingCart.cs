using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Data;

namespace WebApplication2.Models
{
    public class ShoppingCart
    {
        private readonly ApplicationDbContext _appDbContext;
        private ShoppingCart(ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public string ShoppingCartId { get; set; }
        public List<ShoppingCartItemModel> ShoppingCartItems { get; set; }

        public static ShoppingCart GetCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?
                .HttpContext.Session;

            var context = services.GetService<ApplicationDbContext>();
            string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();

            session.SetString("CartId", cartId);

            return new ShoppingCart(context) { ShoppingCartId = cartId };
        }

        public void AddToCart(ProductModel product, int quantity)
        {
            var shoppingCartItem =
                    _appDbContext.ShoppingCartItemModel.SingleOrDefault(
                        s => s.Product.Id == product.Id && s.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItemModel
                {
                    ShoppingCartId = ShoppingCartId,
                    Product = product,
                    Quantity = quantity
                };

                _appDbContext.ShoppingCartItemModel.Add(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.Quantity++;
            }
            _appDbContext.SaveChanges();
        }

        public int RemoveFromCart(ProductModel product)
        {
            var shoppingCartItem =
                    _appDbContext.ShoppingCartItemModel.SingleOrDefault(
                        s => s.Product.Id == product.Id && s.ShoppingCartId == ShoppingCartId);

            var localAmount = 0;

            if (shoppingCartItem != null)
            {
                if (shoppingCartItem.Quantity > 1)
                {
                    shoppingCartItem.Quantity--;
                    localAmount = shoppingCartItem.Quantity;
                }
                else
                {
                    _appDbContext.ShoppingCartItemModel.Remove(shoppingCartItem);
                }
            }

            _appDbContext.SaveChanges();

            return localAmount;
        }



        public List<ShoppingCartItemModel> GetShoppingCartItems()
        {
            var items = ShoppingCartItems ??
                   (ShoppingCartItems =
                       _appDbContext.ShoppingCartItemModel.Where(c => c.ShoppingCartId == ShoppingCartId)
                           .Include(s => s.Product)
                           .ToList());            
            return items;
        }

        public void ClearCart()
        {
            var cartItems = _appDbContext
                .ShoppingCartItemModel
                .Where(cart => cart.ShoppingCartId == ShoppingCartId);

            _appDbContext.ShoppingCartItemModel.RemoveRange(cartItems);

            _appDbContext.SaveChanges();
        }

        public decimal GetShoppingCartTotal()
        {
            var total = _appDbContext.ShoppingCartItemModel.Where(c => c.ShoppingCartId == ShoppingCartId)
                .Select(c => c.Product.Price * c.Quantity).Sum();
            return total;
        }
    }


}
