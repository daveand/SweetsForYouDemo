using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApplication2.Data;
using WebApplication2.Helpers;
using WebApplication2.Interfaces;
using WebApplication2.Models;
using WebApplication2.Models.ViewModels;
using WebApplication2.Services;

namespace WebApplication2.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ShoppingCart _shoppingCart;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;
        private decimal totalSales;
        private decimal totalMargin;
        private bool orderNotReady = false;


        public OrderController(IOrderRepository orderRepository, ShoppingCart shoppingCart, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IEmailService emailService)
        {
            _orderRepository = orderRepository;
            _shoppingCart = shoppingCart;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
        }

        // GET: Orders
        [Authorize(Roles = "Admin")]
        public IActionResult Orders()
        {
            var customers = _context.CustomerModel.Include(p => p.User).Include(p => p.Orders).ToList();

            List<CustomerModel> customersReady = new List<CustomerModel>();
            foreach (var customer in customers)
            {
                //var orders = _context.OrderModel.Include(p => p.User).Include(p => p.OrderLines).Include(p => p.OrderState).Where(m => m.UserId == customer.UserId && m.OrderState.OrderDelivered == false ).ToList();
                var orders = _context.OrderModel
                    .Include(p => p.User)
                    .Include(p => p.OrderLines)
                    .Include(p => p.OrderState)
                    .Where(m => m.UserId == customer.UserId && m.OrderState.OrderDelivered == false )
                    .ToList();

                foreach (var order in orders)
                {
                    orderNotReady = false;
                    if (order.OrderState.OrderSent == false)
                    {
                        orderNotReady = true;
                    }
                }

                if (orderNotReady == false && orders.Count != 0)
                {
                    customersReady.Add(customer);
                }


            }


            //var customers = _context.CustomerModel.Include(p => p.User).Include(p => p.Orders).ToList();

            return View(new OrdersViewModel { Customer = customersReady });
        }

        // GET: Product/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> OrdersDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderModel = await _context.OrderModel
                .Include(p => p.OrderLines)
                .FirstOrDefaultAsync(m => m.Id == id);

            var orderDetailModel = _context.OrderDetailModel
                .Include(p => p.Product)
                .Where(m => m.OrderId == orderModel.Id).ToList();

            var customer = _context.CustomerModel.FirstOrDefault(m => m.UserId == orderModel.UserId);

            if (orderModel == null)
            {
                return NotFound();
            }

            return View(new OrdersDetailsViewModel { Order = orderModel, OrderDetails = orderDetailModel, Customer = customer });
        }

        // GET: Product/Details/5
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> MyOrdersDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderModel = await _context.OrderModel
                .Include(p => p.OrderLines)
                .FirstOrDefaultAsync(m => m.Id == id);

            var orderDetailModel = _context.OrderDetailModel
                .Include(p => p.Product)
                .Where(m => m.OrderId == orderModel.Id).ToList();

            if (orderModel == null)
            {
                return NotFound();
            }

            return View(new OrdersDetailsViewModel { Order = orderModel, OrderDetails = orderDetailModel });
        }


        // GET: MyOrders
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> MyOrders(int? pendingPageIndex, int? deliveredPageIndex)
        {
            string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var allPendingOrders = _context.OrderModel.Include(p => p.OrderLines).Where(m => m.UserId == userId).ToList();
            var orderState = _context.OrderStateModel.ToList();
            int orderQuantity = 0;

            //Get number of participants
            var customer = _context.CustomerModel.FirstOrDefault(c => c.UserId == userId);
            int participants = customer.Participants;

            //Calculate total sales and margin         
            foreach (var item in allPendingOrders)
            {
                if (!item.OrderState.OrderDelivered)
                {
                    orderQuantity++;
                    foreach (var line in item.OrderLines)
                    {
                        var product = _context.ProductModel.FirstOrDefault(m => m.Id == line.ProductId);
                        totalMargin += (line.Quantity * product.Price) * (product.Margin / 100);
                        totalSales += line.Quantity * product.Price;
                    }

                }
            }

            int pendingPageSize = 10;
            int pendingCount = _context.OrderModel.Count();
            var pendingOrders = await PaginatedList<OrderModel>.CreateAsync(
                _context.OrderModel
                .AsNoTracking()
                .Include(p => p.OrderLines)
                .Include(p => p.OrderState)
                .Where(m => m.UserId == userId)
                .Where(m => m.OrderState.OrderDelivered == false)
                .OrderByDescending(o => o.OrderPlaced),
                pendingPageIndex ?? 1, pendingPageSize);

            int deliveredPageSize = 10;
            int deliveredCount = _context.OrderModel.Count();
            var deliveredOrders = await PaginatedList<OrderModel>.CreateAsync(
                _context.OrderModel
                .AsNoTracking()
                .Include(p => p.OrderLines)
                .Include(p => p.OrderState)
                .Where(m => m.UserId == userId)
                .Where(m => m.OrderState.OrderDelivered == true)
                .OrderByDescending(o => o.OrderState.DateDelivered),
                deliveredPageIndex ?? 1, deliveredPageSize);

            return View(new MyOrdersViewModel {
                PendingOrders = pendingOrders,
                DeliveredOrders = deliveredOrders,
                OrderState = orderState,
                TotalSales = (int)totalSales,
                TotalMargin = (int)totalMargin,
                OrderQuantity = orderQuantity,
                Participants = participants
            });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderModel = await _context.OrderModel.FindAsync(id);
            if (orderModel == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", orderModel.UserId);
            return View(orderModel);
        }

        // POST: Customer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,FirstName,LastName,PhoneNumber,Email,OrderTotal,OrderPlaced")] OrderModel orderModel)
        {
            if (id != orderModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderModelExists(orderModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("OrdersDetails", new { id = orderModel.Id });
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", orderModel.UserId);
            return View(orderModel);
        }

        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> OrderLineEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetailModel = await _context.OrderDetailModel.FindAsync(id);
            if (orderDetailModel == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(_context.OrderModel, "Id", "Id", orderDetailModel.OrderId);
            ViewData["ProductId"] = new SelectList(_context.ProductModel, "Id", "Title", orderDetailModel.ProductId);
            return View(orderDetailModel);
        }

        // POST: Customer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderLineEdit(int id, [Bind("Id,OrderId,ProductId,Quantity,Price")] OrderLineEditViewModel orderLineEdit)
        {
            if (id != orderLineEdit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    OrderDetailModel orderDetail = new OrderDetailModel { Id = orderLineEdit.Id };   /// stub model, only has Id
                    _context.OrderDetailModel.Attach(orderDetail); /// track your stub model
                    _context.Entry(orderDetail).CurrentValues.SetValues(orderLineEdit); /// reflection
                    //_context.Update(orderLineEdit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderModelExists(orderLineEdit.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("OrdersDetails", new { id = orderLineEdit.OrderId });
            }
            //ViewData["OrderId"] = new SelectList(_context.OrderModel, "Id", "Id", orderDetailModel.OrderId);
            //ViewData["ProductId"] = new SelectList(_context.ProductModel, "Id", "Title", orderDetailModel.ProductId);
            return View(orderLineEdit);
        }




        [Authorize]
        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Checkout(OrderModel order)
        {
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;
            if (_shoppingCart.ShoppingCartItems.Count == 0)
            {
                ModelState.AddModelError("", "Din kundvagn är tom! Lägg till produkter.");
            }

            if (ModelState.IsValid)
            {
                _orderRepository.CreateOrder(order);
                _shoppingCart.ClearCart();
                return RedirectToAction("CheckoutComplete");
            }

            return View(order);
        }

        public IActionResult CheckoutComplete()
        {
            ViewBag.CheckoutCompleteMessage = "Tack för din order!";
            return View();
        }

        [Authorize]
        public IActionResult SendOrder()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult SendOrder(OrderModel order)
        {
            string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var orders = _context.OrderModel.AsNoTracking().Include(m => m.User).Where(m => m.UserId == userId && m.OrderState.OrderSent == false).ToList();

            var details = _context.OrderDetailModel.Where(m => m.Order.UserId == userId).ToList();

            foreach (var item in orders)
            {

                var result = _context.OrderStateModel.SingleOrDefault(b => b.OrderId == item.Id);
                if (result != null)
                {
                    result.OrderSent = true;
                    _context.SaveChanges();
                    _emailService.SendOrderConfirmation(item);
                }

            }


            //return View(orders);
            return View(new SendOrderViewModel { Orders = orders, Details = details });
        }

        [Authorize]
        public IActionResult DeliverOrder()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult DeliverOrder(int id, OrderModel order)
        {
            var customer = _context.CustomerModel.Include(m => m.User).FirstOrDefault(m => m.Id == id);
            var orders = _context.OrderModel.AsNoTracking().Include(m => m.OrderLines).Where(m => m.UserId == customer.UserId && m.OrderState.OrderDelivered == false).ToList();
            //var details = _context.OrderDetailModel.Where(m => m.Order.UserId == userId).ToList();
            int orderTotal = 0;

            foreach (var item in orders)
            {
                var result = _context.OrderStateModel.SingleOrDefault(b => b.OrderId == item.Id);
                if (result != null)
                {
                    result.OrderSent = true;
                    result.OrderDelivered = true;
                    result.DateDelivered = DateTime.Now.ToString();
                    _context.SaveChanges();
                    //_emailService.SendOrderConfirmation(item);
                }
            }

            foreach (var item in orders)
            {
                foreach (var line in item.OrderLines)
                {
                    orderTotal += (int)(line.Quantity * line.Price);
                }
            }

            _emailService.SendDeliverConfirmation(customer, orderTotal);
            orderTotal = 0;
            return RedirectToAction("Details", "Customer", new { id = id });
        }


        // GET: Orders
        public IActionResult GetOrders()
        {
            var orders = _context.OrderModel.Include(p => p.OrderState).ToList();
            var customers = _context.CustomerModel.Include(p => p.Orders).ToList();

            List<OrdersDataTableModel> ordersData = new List<OrdersDataTableModel>();
            List<OrderDetailModel> orderDetails = new List<OrderDetailModel>();

            foreach (var item in orders)
            {

                var customer = customers.FirstOrDefault(m => m.UserId == item.UserId);

                ordersData.Add(new OrdersDataTableModel
                {
                    Id = item.Id,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    OrderPlaced = item.OrderPlaced.ToString("yyyy-MM-dd HH:MM"),
                    OrderState = item.OrderState.OrderSent,
                    CustomerId = customer.Id,
                    Company = customer.Company,
                    Department = customer.Department
                });

            };

            //return Json(new OrdersViewModel { Order = orders, Customer = customers });
            return Json(new { data = ordersData });

        }

        // GET: Customer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderModel = await _context.OrderModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderModel == null)
            {
                return NotFound();
            }

            var customerModel = await _context.CustomerModel.FirstOrDefaultAsync(m => m.UserId == orderModel.UserId);

            return View(new DeleteOrderViewModel { Order = orderModel, Customer = customerModel });
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int customerId)
        {
            var orderModel = await _context.OrderModel.FindAsync(id);
            _context.OrderModel.Remove(orderModel);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Customer", new { id = customerId });
        }
        // GET: Customer/Delete/5
        public async Task<IActionResult> MyOrdersDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderModel = await _context.OrderModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderModel == null)
            {
                return NotFound();
            }

            return View(orderModel);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("MyOrdersDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyOrdersDeleteConfirmed(int id)
        {
            var orderModel = await _context.OrderModel.FindAsync(id);
            _context.OrderModel.Remove(orderModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyOrders));
        }


        // GET: Customer/Delete/5
        public async Task<IActionResult> OrderLineDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetailModel = await _context.OrderDetailModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderDetailModel == null)
            {
                return NotFound();
            }

            return View(orderDetailModel);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("OrderLineDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderLineDeleteConfirmed(int id)
        {
            var orderDetailModel = await _context.OrderDetailModel.FindAsync(id);
            _context.OrderDetailModel.Remove(orderDetailModel);
            await _context.SaveChangesAsync();
            return RedirectToAction("OrdersDetails", new { id = orderDetailModel.OrderId });
        }



        private bool OrderModelExists(int id)
        {
            return _context.OrderModel.Any(e => e.Id == id);
        }



    }
}