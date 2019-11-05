using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Helpers;
using WebApplication2.Models;
using WebApplication2.Models.ViewModels;

namespace WebApplication2.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Product
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int? pageIndex)
        {
            int pageSize = 10;
            int count = _context.OrderModel.Count();
            var applicationDbContext = await PaginatedList<ProductModel>.CreateAsync(
                _context.ProductModel.AsNoTracking().Include(p => p.Category).OrderBy(m => m.Title),
                pageIndex ?? 1, pageSize);

            return View(new ProductListViewModel
            {
                Products = applicationDbContext
            });
        }

        // GET: Product
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> ProductShop(int? pageIndex)
        {
            int pageSize = 10;
            int count = _context.OrderModel.Count();
            var applicationDbContext = await PaginatedList<ProductModel>.CreateAsync(
                _context.ProductModel.AsNoTracking().Include(p => p.Category).OrderBy(m => m.Title),
                pageIndex ?? 1, pageSize);

            return View(new ProductListViewModel
            {
                Products = applicationDbContext
            });
        }

        // GET: Product
        [AllowAnonymous]
        public async Task<IActionResult> ProductList(int? pageIndex)
        {
            int pageSize = 10;
            int count = _context.OrderModel.Count();
            var applicationDbContext = await PaginatedList<ProductModel>.CreateAsync(
                _context.ProductModel.AsNoTracking().Include(p => p.Category).OrderBy(m => m.Title),
                pageIndex ?? 1, pageSize);
            
            //return View(applicationDbContext);
            return View(new ProductListViewModel
            {
                Products = applicationDbContext
            });
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.ProductModel
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productModel == null)
            {
                return NotFound();
            }

            return View(productModel);
        }

        // GET: Product/Details/5
        [HttpGet]
        public async Task<IActionResult> Information(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.ProductModel
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productModel == null)
            {
                return NotFound();
            }

            return PartialView("_InformationPartial", productModel);
        }


        // GET: Product/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Set<CategoryModel>(), "Id", "Category");
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,SubTitle,Description,Contents,CategoryId,Price,Margin,ImageFile,ImageName,ImageDescription")] ProductModel productModel, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                var filename = ContentDispositionHeaderValue.Parse(ImageFile.ContentDisposition).FileName.Trim('"');
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", ImageFile.FileName);
                using (System.IO.Stream stream = new FileStream(path, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
                productModel.ImageFile = filename;
                _context.Add(productModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Set<CategoryModel>(), "Id", "Id", productModel.CategoryId);
            return View(productModel);
        }

        // GET: Product/Create
        public IActionResult CreateCategory()
        {
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory([Bind("Id, Category")] CategoryModel categoryModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoryModel);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Create");
        }


        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.ProductModel.FindAsync(id);

            if (productModel == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Set<CategoryModel>(), "Id", "Category", productModel.CategoryId);
            return View(productModel);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,SubTitle,Description,Contents,Price,Margin,CategoryId,ImageFile,ImageName,ImageDescription")] ProductModel productModel, IFormFile ImageFile)
        {
            if (id != productModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if new image is uploaded
                    if (ImageFile != null)
                    {
                        var filename = ContentDispositionHeaderValue.Parse(ImageFile.ContentDisposition).FileName.Trim('"');
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", ImageFile.FileName);
                        using (System.IO.Stream stream = new FileStream(path, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }
                        productModel.ImageFile = filename;
                    }
                    // If not, get last filename and save it again.
                    else
                    {
                        var product = _context.ProductModel.AsNoTracking().Where(x => x.Id == productModel.Id).FirstOrDefault();

                        productModel.ImageFile = product.ImageFile;

                    }

                    _context.Update(productModel);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductModelExists(productModel.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Set<CategoryModel>(), "Id", "Id", productModel.CategoryId);
            return View(productModel);
        }

       
        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.ProductModel
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productModel == null)
            {
                return NotFound();
            }

            return View(productModel);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productModel = await _context.ProductModel.FindAsync(id);
            _context.ProductModel.Remove(productModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductModelExists(int id)
        {
            return _context.ProductModel.Any(e => e.Id == id);
        }
    }
}
