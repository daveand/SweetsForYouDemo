using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Models.ViewModels;
using WebApplication2.Services.PDFService;

namespace WebApplication2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPDFService _pdfService;
        private decimal orderTotal;
        private decimal totalMargin;
        private int countQuantity;
        private int subTotal;



        public CustomerController(ApplicationDbContext context, IPDFService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

        // GET: Customer
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CustomerModel.Include(c => c.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Customers
        public IActionResult GetCustomers()
        {
            var customers = _context.CustomerModel.Include(p => p.User).ToList();

            List<CustomersDataTableModel> customersData = new List<CustomersDataTableModel>();

            foreach (var item in customers)
            {
                customersData.Add(new CustomersDataTableModel
                {
                    Id = item.Id,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Company = item.Company,
                    Department = item.Department,
                    Email = item.User.Email,
                    Telephone = item.Telephone
                });

            };

            //return Json(new OrdersViewModel { Order = orders, Customer = customers });
            return Json(new { data = customersData });

        }


        // GET: Customer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Get customer info
            var customerModel = await _context.CustomerModel
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customerModel == null)
            {
                return NotFound();
            }
            var orderModel = _context.OrderModel
                .Include(c => c.User)
                .Include(p => p.OrderState)
                .Include(o => o.OrderLines)
                .Where(m => m.UserId == customerModel.UserId)
                .OrderByDescending(m => m.OrderState.DateDelivered)
                .ToList();
            if (orderModel == null)
            {
                return NotFound();
            }

            List<OrderSummaryViewModel> orderSummary = new List<OrderSummaryViewModel>();
            List<int> productIdList = new List<int>();

            foreach (var item in orderModel)
            {
                if (!item.OrderState.OrderDelivered)
                {
                    foreach (var line in item.OrderLines)
                    {
                        if (!productIdList.Contains(line.ProductId))
                        {
                            productIdList.Add(line.ProductId);

                        }
                        var product = _context.ProductModel.FirstOrDefault(m => m.Id == line.ProductId);
                        orderTotal += line.Quantity * line.Price;
                        totalMargin += (line.Quantity * line.Price) * (product.Margin / 100);
                    }

                }
            }

            foreach (var item in productIdList)
            {
                var productDetails = _context.ProductModel.Include(m => m.Category).FirstOrDefault(m => m.Id == item);
                foreach (var order in orderModel)
                {
                    if (!order.OrderState.OrderDelivered)
                    {
                        foreach (var line in order.OrderLines)
                        {
                            if (line.ProductId == productDetails.Id)
                            {
                                countQuantity += line.Quantity;
                                subTotal += (int)(line.Quantity * line.Price);

                            }
                        }

                    }
                }

                orderSummary.Add(new OrderSummaryViewModel
                {
                    ProductTitle = productDetails.Title,
                    Category = productDetails.Category.Category,
                    MarginPercent = (int)productDetails.Margin,
                    Quantity = countQuantity,
                    SubTotal = subTotal,
                    Margin = (subTotal * (productDetails.Margin / 100))
                });

                countQuantity = 0;
                subTotal = 0;

            }

            CustomerInfoModel customerInfoModel = new CustomerInfoModel();
            customerInfoModel.CustomerModel = customerModel;
            customerInfoModel.Orders = orderModel;
            customerInfoModel.OrderTotal = (int)orderTotal;
            customerInfoModel.TotalMargin = (int)totalMargin;
            customerInfoModel.OrderSummary = orderSummary;

            return View(customerInfoModel);
        }

        // GET: Customer/Create
        public IActionResult Create(int? id)
        {

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Customer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? id, [Bind("Id,UserId,FirstName,LastName,Telephone,Company,Department,Street,PostalCode,City,PersonalNumber,Participants,Receiver")] CustomerModel customerModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customerModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", customerModel.UserId);
            return View(customerModel);
        }

        // GET: Customer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerModel = await _context.CustomerModel.FindAsync(id);
            if (customerModel == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", customerModel.UserId);
            return View(customerModel);
        }

        // POST: Customer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,FirstName,LastName,Telephone,Company,Department,Street,PostalCode,City,PersonalNumber,Participants,Receiver")] CustomerModel customerModel)
        {
            if (id != customerModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customerModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerModelExists(customerModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", customerModel.UserId);
            return View(customerModel);
        }

        // GET: Customer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerModel = await _context.CustomerModel
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customerModel == null)
            {
                return NotFound();
            }

            return View(customerModel);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customerModel = await _context.CustomerModel.FindAsync(id);
            _context.CustomerModel.Remove(customerModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> GenerateOrderPdf(int? id)
        {

            //Get customer info
            var customerModel = await _context.CustomerModel
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            var orderModel = _context.OrderModel
                .Include(c => c.User)
                .Include(p => p.OrderState)
                .Include(o => o.OrderLines)
                .Where(m => m.UserId == customerModel.UserId && m.OrderState.OrderDelivered != true)
                .OrderByDescending(m => m.OrderState.DateDelivered)
                .ToList();

            List<OrderSummaryViewModel> orderSummary = new List<OrderSummaryViewModel>();
            List<int> productIdList = new List<int>();

            foreach (var item in orderModel)
            {
                if (!item.OrderState.OrderDelivered)
                {
                    foreach (var line in item.OrderLines)
                    {
                        if (!productIdList.Contains(line.ProductId))
                        {
                            productIdList.Add(line.ProductId);

                        }
                        var product = _context.ProductModel.FirstOrDefault(m => m.Id == line.ProductId);
                        orderTotal += line.Quantity * line.Price;
                        totalMargin += (line.Quantity * line.Price) * (product.Margin / 100);
                    }

                }
            }

            foreach (var item in productIdList)
            {
                var productDetails = _context.ProductModel.Include(m => m.Category).FirstOrDefault(m => m.Id == item);
                foreach (var order in orderModel)
                {
                    if (!order.OrderState.OrderDelivered)
                    {
                        foreach (var line in order.OrderLines)
                        {
                            if (line.ProductId == productDetails.Id)
                            {
                                countQuantity += line.Quantity;
                                subTotal += (int)(line.Quantity * line.Price);

                            }
                        }

                    }
                }

                orderSummary.Add(new OrderSummaryViewModel
                {
                    ProductTitle = productDetails.Title,
                    Category = productDetails.Category.Category,
                    MarginPercent = (int)productDetails.Margin,
                    Quantity = countQuantity,
                    SubTotal = subTotal,
                    Margin = (subTotal * (productDetails.Margin / 100))
                });

                countQuantity = 0;
                subTotal = 0;

            }



            var file = await _pdfService.Create(new OrdersPdfModel {
                Customer = customerModel,
                OrderTotal = (int)orderTotal,
                TotalMargin = (int)totalMargin, 
                OrderSummary = orderSummary,
                Orders = orderModel
            });
            return File(file, "application/pdf", "Plocklista_" + customerModel.Id + "_" + customerModel.Company + "_" + customerModel.Department + ".pdf");
        }

        private bool CustomerModelExists(int id)
        {
            return _context.CustomerModel.Any(e => e.Id == id);
        }
    }
}
