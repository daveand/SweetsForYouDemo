using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Data;
using WebApplication2.Models;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication2.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ShoppingCart _shoppingCart;

        public ShoppingCartController(ApplicationDbContext context, ShoppingCart shoppingCart)
        {
            _context = context;
            _shoppingCart = shoppingCart;
        }

        [Authorize]
        public ViewResult Index()
        {
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            var shoppingCartViewModel = new ShoppingCartViewModel
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal()
            };
            return View(shoppingCartViewModel);
        }

        [Authorize]
        [HttpPost]
        public RedirectToActionResult AddToShoppingCart(int id, [Bind("Quantity")] ProductModel productModel)
        {
            int quantity = productModel.Quantity;
            var selectedDrink = _context.ProductModel.FirstOrDefault(p => p.Id == id);
            if (selectedDrink != null)
            {
                _shoppingCart.AddToCart(selectedDrink, quantity);
            }
            return RedirectToAction("ProductShop", "Product");
        }

        public RedirectToActionResult RemoveFromShoppingCart(int productId)
        {
            var selectedDrink = _context.ProductModel.FirstOrDefault(p => p.Id == productId);
            if (selectedDrink != null)
            {
                _shoppingCart.RemoveFromCart(selectedDrink);
            }
            return RedirectToAction("Index");
        }


    }
}
