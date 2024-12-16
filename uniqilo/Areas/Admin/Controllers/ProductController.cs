using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using uniqilo.DataAcces;
using uniqilo.Extension;
using uniqilo.Models;
using uniqilo.ViewModel.Product;

namespace uniqilo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController(AppDbContext _context, IWebHostEnvironment _env) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.Include(x => x.Category).ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();
            return View();
        }
        [HttpPost]

        public async Task<IActionResult> Create(ProductCreateVM vm)
        {
            if (vm.CoverImage != null)
            {
                if (!vm.CoverImage.ContentType.StartsWith("image"))
                {
                    ModelState.AddModelError("File", "File must be image!");
                }
                if (vm.CoverImage.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("File", "File must be less than 5mb!");
                }
            }            
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();
                return View();
            }

            Product product = new Product
            {
                ProductName = vm.ProductName,
                ProductDescription = vm.ProductDescription,
                CostPrice = vm.CostPrice,
                SellPrice = vm.SellPrice,
                Discount = vm.Discount,
                Quantity = vm.Quantity,
                CategoryID = vm.CategoryID,
                CoverImage = await vm.CoverImage!.UploadAsync(_env.WebRootPath, "img", "products"),
                
            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue) return BadRequest();
            var data = await _context.Products.FindAsync(id);
            if (data is null) return NotFound();
            _context.Products.Remove(data);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Hide(int? id, ProductCreateVM vm)
        {
            var data = await _context.Products.FindAsync(id);
            if (data is null) return View();
            data.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Show(int? id, ProductCreateVM vm)
        {
            var data = await _context.Products.FindAsync(id);
            if (data is null) return View();
            data.IsDeleted = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();
            if (!id.HasValue) return BadRequest();
            var data = await _context.Products
                .Where(p => p.Id == id.Value)
                .Select(x => new ProductUpdateVM
                {
                    ProductName = x.ProductName,
                    ProductDescription = x.ProductDescription,
                    CostPrice = x.CostPrice,
                    SellPrice = x.SellPrice,
                    Discount = x.Discount,
                    Quantity = x.Quantity,
                    CategoryID = x.CategoryID,
                    ImageUrl = x.CoverImage,
                    
                }).FirstOrDefaultAsync();
            if (data is null) return NotFound();
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, ProductUpdateVM vm)
        {
            if (!id.HasValue) return BadRequest();
            if (vm.CoverImage != null)
            {
                if (!vm.CoverImage.ContentType.StartsWith("image"))
                    ModelState.AddModelError("File", "File type must be an image");
                if (vm.CoverImage.Length > 5 * 1024 * 1024)
                    ModelState.AddModelError("File", "File must be less than 5mb");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();
                return View(vm);
            }
            var products = await _context.Products
                .Where(p => p.Id == id.Value)
                .FirstOrDefaultAsync();
            if (products is null) return NotFound();
            if (vm.CoverImage != null)
            {
                string path = Path.Combine(_env.WebRootPath, "img", "products", products.CoverImage);
                using (Stream sr = System.IO.File.Create(path))
                {
                    await vm.CoverImage!.CopyToAsync(sr);
                }
            }

            products.ProductName = vm.ProductName;
            products.ProductDescription = vm.ProductDescription;
            products.CostPrice = vm.CostPrice;
            products.SellPrice = vm.SellPrice;
            products.Quantity = vm.Quantity;
            products.Discount = vm.Discount;
            products.CategoryID = vm.CategoryID;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}