using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Models.ViewModels;

namespace WebApplication2.Controllers
{
    public class GetOrdersModel
    {
        public string Product { get; set; }
        public int OrderQty { get; set; }
    }
    public class GetSalesModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Amount { get; set; }
    }
    public class YearMonth
    {
        public int Year { get; set; }
        public int Month { get; set; }
    }

    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {

            var orders = await _context.OrderModel
                .Include(m => m.OrderLines)
                .OrderBy(m => m.OrderPlaced)
                .ToListAsync();

            List<int> yearArray = new List<int>();

            foreach (var order in orders)
            {
                if (!yearArray.Contains(order.OrderPlaced.Year))
                {
                    yearArray.Add(order.OrderPlaced.Year);
                }


            }

            ViewBag.SalesYears = new SelectList(yearArray);
            return View();
        }


        public async Task<JsonResult> GetOrders()
        {
            var orders = await _context.OrderModel
                .Include(m => m.OrderLines)
                .Include(m => m.Product).ToListAsync();

            int totalOrders = orders.Count();
            int ordersPerProduct = 0;

            List<int> productArray = new List<int>();

            foreach (var order in orders)
            {
                foreach (var line in order.OrderLines)
                {
                    if (!productArray.Contains(line.ProductId))
                    {
                        productArray.Add(line.ProductId);
                    }

                }
            }

            List<GetOrdersModel> orderStatistics = new List<GetOrdersModel>();

            foreach (var product in productArray)
            {
                foreach (var order in orders)
                {
                    foreach (var line in order.OrderLines)
                    {
                        if (line.ProductId == product)
                        {
                            ordersPerProduct += line.Quantity;
                        }
                    }
                }
                var productName = _context.ProductModel.FirstOrDefault(m => m.Id == product);
                orderStatistics.Add(new GetOrdersModel { Product = productName.Title, OrderQty = ordersPerProduct });

                ordersPerProduct = 0;
            }

            return Json(orderStatistics);
        }

        public async Task<JsonResult> GetSales()
        {

            var orders = await _context.OrderModel
                .Include(m => m.OrderLines)
                .OrderBy(m => m.OrderPlaced)
                .ToListAsync();

            int amountPerMonth = 0;

            List<YearMonth> yearMonths = new List<YearMonth>();

            foreach (var order in orders)
            {
                if (!yearMonths.Any(m => m.Year == order.OrderPlaced.Year && m.Month == order.OrderPlaced.Month))
                {
                    yearMonths.Add(new YearMonth { Year = order.OrderPlaced.Year, Month = order.OrderPlaced.Month } );

                }
            }

            List<GetSalesModel> salesStatistics = new List<GetSalesModel>();


            foreach (var yearMonth in yearMonths)
            {
                foreach (var order in orders)
                {
                    if (order.OrderPlaced.Year == yearMonth.Year && order.OrderPlaced.Month == yearMonth.Month)
                    {
                        foreach (var line in order.OrderLines)
                        {
                            amountPerMonth += (int)(line.Price * line.Quantity);
                        }
                    }
                }
 
                salesStatistics.Add(new GetSalesModel { Year = yearMonth.Year, Month = yearMonth.Month, Amount = amountPerMonth });

                amountPerMonth = 0;
            }

            return Json(salesStatistics);
        }
    }
}