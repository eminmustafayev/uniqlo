using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using uniqilo.DataAcces;
using uniqilo.Models;
using uniqilo.ViewModel.Category;

namespace uniqilo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController(AppDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateVM vm)
        {
            if (!ModelState.IsValid) return View();


            Category category = new Category
            {
                CategoryName = vm.Name
            };

            await _context.AddAsync(category);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var data = await _context.Categories.FindAsync(id);

            if (data is null) return NotFound();

            CategoryUpdateVM vm = new();

            vm.Name = data.CategoryName;

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, CategoryUpdateVM vm)
        {

            if (!id.HasValue) return BadRequest();
            if (!ModelState.IsValid) return View();

            var data = await _context.Categories.FindAsync(id);

            if (data is null) return View();

            data.CategoryName = vm.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();
            var data = await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
            if (data is null) return View();

            if (data.Products.Any())
            {
                TempData["ErrorMessage"] = "Bu kateqoriya ile bagli mehsullar movcuddur.Ona gore bu kateqoriyani sile bilmersiniz.";
                return RedirectToAction(nameof(Index));
            }
            _context.Categories.Remove(data);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Kateqoriya ugurla silindi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Hide(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var data = await _context.Categories.FindAsync(id);

            if (data is null) return View();

            data.IsDeleted = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Show(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var data = await _context.Categories.FindAsync(id);

            if (data is null) return View();

            data.IsDeleted = false;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
